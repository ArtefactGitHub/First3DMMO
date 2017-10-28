using com.Artefact.First3DMMO.WorkSpace.ConnectNetwork;
using com.Artefact.FrameworkNetwork.Cores;
using System;
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

			// サーバーからの直接メッセージ受信
			PushMessageManager.Instance.PushMessageAsObservable.Subscribe(messageData => SyncPlayer(messageData)).AddTo(this);

			InitializeStage();

			InitializeNetwork();
		}

		private void SyncPlayer(IMessageData message)
		{
			Debug.Log("sync : " + message.Result);
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
			Observable.FromCoroutine<Exception>(observer => NetworkManager.Instance.Initialize(observer, SampleDefine.EndPoint))
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
	}
}