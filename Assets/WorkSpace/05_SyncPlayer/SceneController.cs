using com.Artefact.First3DMMO.WorkSpace.ConnectNetwork;
using com.Artefact.FrameworkNetwork.Cores;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

			// サーバーからの直接メッセージ受信
			PushMessageManager.Instance.PushMessageAsObservable.Subscribe(messageData => SyncPlayer(messageData)).AddTo(this);

			InitializeStage();

			InitializeNetwork();
		}

		private List<ResponseSyncPlayer> m_Players = new List<ResponseSyncPlayer>();

		private void SyncPlayer(IMessageData message)
		{
			Debug.Log(string.Format("[{0}] : {1}", message.CommandName, message.Result));

			if(message.CommandName.Equals("syncPlayer"))
			{
				ResponseSyncPlayer player = new ResponseSyncPlayer();
				player.TryParse(message.Result);

				m_Players.Add(player);
			}
			else if(message.CommandName.Equals("disconnectPlayer"))
			{
				ResponseSyncPlayer player = new ResponseSyncPlayer();
				player.TryParse(message.Result);

				foreach(var x in m_Players)
				{
					if(x.PlayerIndex == player.PlayerIndex)
					{
						m_Players.Remove(player);
					}
				}
			}
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

					Debug.Log("connect success");

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