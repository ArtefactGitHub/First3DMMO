using com.Artefact.FrameworkNetwork.Cores;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public class SampleNetworkManager : MonoBehaviour
	{
		#region singleton
		private static SampleNetworkManager _Instance = null;

		public static SampleNetworkManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = FindObjectOfType<SampleNetworkManager>();
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
			Log("SampleNetworkManager.OnOpen()");
		}

		public void OnMessage(string message)
		{
			Log("SampleNetworkManager.OnMessage()\n" + message);
		}

		public void OnError(string message)
		{
			Log("SampleNetworkManager.OnError()\n" + message);
		}

		public void OnClose()
		{
			Log("SampleNetworkManager.OnClose()");
		}

		private void Log(string str)
		{
			Debug.Log(string.Format("<color=green>{0}</color>", str));
		}
	}
}