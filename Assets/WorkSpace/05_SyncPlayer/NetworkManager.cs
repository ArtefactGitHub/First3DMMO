using com.Artefact.FrameworkNetwork.Cores;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class NetworkManager : MonoBehaviour
	{
		#region singleton
		private static NetworkManager _Instance = null;

		public static NetworkManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = FindObjectOfType<NetworkManager>();
				}
				return _Instance;
			}
		}
		#endregion

		public IEnumerator Initialize(IObserver<Exception> observer, string endPoint)
		{
			ConnectionParameter param = new ConnectionParameter(endPoint, OnOpen, OnMessage, OnError, OnClose);

			yield return Connection.Instance.Initialize(param).StartAsCoroutine((ex) =>
			{
				observer.OnNext(ex);
			},
			(ex) =>
			{
				observer.OnNext(ex);
			});
		}

		public void OnOpen()
		{
			Log("NetworkManager.OnOpen()");
		}

		public void OnMessage(IMessageData message)
		{
			Log("NetworkManager.OnMessage()\n" + message.ToString());

			PushMessageManager.Instance.OnMessage(message);
		}

		public void OnError(string message)
		{
			Log("NetworkManager.OnError()\n" + message.ToString());
		}

		public void OnClose()
		{
			Log("NetworkManager.OnClose()");
		}

		private void Log(string str)
		{
			Debug.Log(string.Format("<color=green>{0}</color>", str));
		}
	}
}