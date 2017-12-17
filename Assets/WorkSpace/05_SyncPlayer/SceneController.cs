using com.Artefact.First3DMMO.WorkSpace.ConnectNetwork;
using com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere;
using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class SceneController : MonoBehaviour
	{
		[SerializeField]
		private bool IsPlayerPrefsClear = true;

		[SerializeField]
		private GameObject m_RootSpawn = null;

		[SerializeField]
		private GameObject m_PrefabSpawn = null;

		[SerializeField, Range(1, 100)]
		private int m_SpawnCount = 100;

		[SerializeField]
		private Vector3 m_SpawnLeftFar = new Vector3(-90.0f, 0f, 90.0f);

		[SerializeField]
		private Vector3 m_SpawnRightNear = new Vector3(90.0f, 0f, -90.0f);

		private IObjectSpawnController m_SpawnController = null;

		private void Start()
		{
			if(IsPlayerPrefsClear)
			{
				SamplePlayerPrefs.DeleteAll();
			}

			m_PlayerController = FindObjectOfType<PlayerController>();

			// サーバーからの直接メッセージ受信
			PushMessageManager.Instance.PushMessageAsObservable.Subscribe(messageData => SyncPlayer(messageData)).AddTo(this);

			InitializeStage();

			InitializeNetwork();
		}

		private PlayerController m_PlayerController = null;

		private int PlayerIndex { get; set; }

		private List<ISyncPlayerData> m_Players = new List<ISyncPlayerData>();

		[SerializeField]
		private GameObject m_PrefabPlayerObject = null;

		private List<PlayerObject> m_PlayerObjects = new List<PlayerObject>();

		private void AddPlayer(List<ISyncPlayerData> addPlayerDatas)
		{
			foreach(var data in addPlayerDatas)
			{
				if(!m_Players.Any(x => x.PlayerIndex == data.PlayerIndex) && data.PlayerIndex != PlayerIndex)
				{
					m_Players.Add(data);

					PlayerObject obj = Instantiate(m_PrefabPlayerObject, Vector3.zero, Quaternion.identity).GetComponent<PlayerObject>();
					obj.Initialize(data);

					m_PlayerObjects.Add(obj);
				}
			}
		}

		private void UpdatePlayer(List<ISyncPlayerData> playerDatas)
		{
			foreach(var data in playerDatas)
			{
				PlayerObject obj = m_PlayerObjects.FirstOrDefault(x => x.PlayerData.PlayerIndex == data.PlayerIndex);
				if(obj != null)
				{
					obj.UpdateData(data);
				}
			}
		}

		private void SyncPlayer(IMessageData message)
		{
			Debug.Log(string.Format("<< [{0}] : {1}", message.CommandName, message.Result));

			if(message.CommandName.Equals("syncPlayer"))
			{
				ResponseSyncPlayer player = new ResponseSyncPlayer();
				player.TryParse(message.Result);

				AddPlayer(new List<ISyncPlayerData>() { player.SyncPlayerData });

				LogOtherPlayers("syncPlayer");
			}
			else if(message.CommandName.Equals("syncOtherPlayers"))
			{
				Debug.LogWarningFormat("syncOtherPlayers");

				ResponseSyncOtherPlayers response = new ResponseSyncOtherPlayers();
				response.TryParse(message.Result);

				// インデックスの取得
				PlayerIndex = response.PlayerIndex;

				AddPlayer(response.SyncPlayerDatas);

				LogOtherPlayers("syncOtherPlayers");
			}
			else if(message.CommandName.Equals("updateOtherPlayers"))
			{
				ResponseUpdateOtherPlayers response = new ResponseUpdateOtherPlayers();
				response.TryParse(message.Result);

				UpdatePlayer(response.SyncPlayerDatas);

				Debug.Log("update");
				//LogOtherPlayers("updateOtherPlayers");
			}
			else if(message.CommandName.Equals("disconnectPlayer"))
			{
				Debug.LogWarningFormat("disconnectPlayer");

				ResponseSyncPlayer response = new ResponseSyncPlayer();
				response.TryParse(message.Result);

				RemovePlayer(response.SyncPlayerData);

				LogOtherPlayers("disconnectPlayer");
			}
		}

		private void RemovePlayer(ISyncPlayerData playerData)
		{
			Debug.LogWarning("RemovePlayer() playerData : " + playerData.ToString());

			ISyncPlayerData data = m_Players.FirstOrDefault(x => x.PlayerIndex == playerData.PlayerIndex);
			if(data != null)
			{
				m_Players.Remove(data);

				PlayerObject targetObj = m_PlayerObjects.FirstOrDefault(obj => obj.PlayerData.PlayerIndex == playerData.PlayerIndex);
				if(targetObj != null)
				{
					m_PlayerObjects.Remove(targetObj);

					Destroy(targetObj.gameObject);
				}
			}
		}

		private void LogOtherPlayers(string title = "")
		{
			string s = string.Format("=== {0} ===\n", title);
			foreach(var x in m_Players)
			{
				s += string.Format("[{0}]\n", x.ToString());
			}
			Debug.Log(s);
		}

		private void InitializeStage()
		{
			m_SpawnController = ObjectSpawnController.Instance;
			for(int i = 0; i < m_SpawnCount; i++)
			{
				m_SpawnController.SpawnRandomArea(m_RootSpawn, m_PrefabSpawn, m_SpawnLeftFar, m_SpawnRightNear);
			}
		}

		private void InitializeNetwork()
		{
			Observable.FromCoroutine<Exception>(observer => NetworkManager.Instance.Initialize(observer, GetEndpoint()))
				.Subscribe(ex =>
				{
					// エラーの場合、エラーメッセージを表示する
					if(ex != null)
					{
						SampleErrorManager.Instance.SetMessage(ex.Message);
						return;
					}

					Debug.Log("InitializeNetwork() success");

					SetConnectUpdatePosition();

				}).AddTo(this);
		}

		private void SetConnectUpdatePosition()
		{
			PlayerModule playerModule = new PlayerModule();
			Observable.IntervalFrame(3).Subscribe(_ =>
			{
				SyncPlayerData data = new SyncPlayerData();
				data.Setup(PlayerIndex, m_PlayerController.transform.position.x, m_PlayerController.transform.position.z);

				playerModule.UpdatePosition(data).Subscribe(result =>
				{
					if(result.Exception != null)
					{
						SampleErrorManager.Instance.SetMessage(result.Exception.Message);
					}
				}).AddTo(this);
			}).AddTo(this);
		}

		/// <summary>
		/// 接続先の取得
		/// 
		/// 接続先設定用JSONファイルが存在する場合、下記パラメータを取得します。
		/// 存在しない場合、デフォルトの値を返します。
		/// 　・"host" : 接続先URL
		/// 　・"port" : 接続先ポート番号
		/// </summary>
		/// <returns></returns>
		private string GetEndpoint()
		{
			string result = "";

			TextAsset asset = Resources.Load<TextAsset>("Config/~ServerSettings");
			if(asset != null)
			{
				try
				{
					JObject obj = JObject.Parse(asset.text);
					result = string.Format("{0}:{1}", obj["host"], obj["port"]);
				}
				catch(Exception e)
				{
					Debug.LogWarning("can not parse ServerSettings");
				}

				Debug.Log(string.Format("loaded ServerSettings=[{0}]", result ?? ""));
			}

			return (string.IsNullOrEmpty(result) ? SampleDefine.EndPoint : result);
		}
	}
}