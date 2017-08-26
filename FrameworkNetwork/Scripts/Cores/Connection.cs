using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using UniRx;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public interface IConnection
	{
		IObservable<Exception> Initialize(ConnectionParameter param);

		void Send(JObject obj);
	}

	public class Connection : MonoBehaviour, IConnection
	{
		#region singleton
		private static Connection m_Instance = null;

		public static IConnection Instance
		{
			get
			{
				if(m_Instance == null)
				{
					m_Instance = FindObjectOfType<Connection>();
				}
				return m_Instance;
			}
		}
		#endregion

		private WebSocket m_Socket = null;

		public IObservable<Exception> Initialize(ConnectionParameter param)
		{
			return Observable.FromCoroutine<Exception>(observer => ConnectRoutine(observer, param)).SelectMany(ex =>
			{
				return Observable.Return(ex);
			});
		}

		private ConnectionParameter _Param = null;

		private bool _IsConnected { get; set; }

		private Exception ConnectionError { get; set; }

		public IEnumerator ConnectRoutine(IObserver<Exception> observer, ConnectionParameter param)
		{
			if(param == null || !param.IsValid())
			{
				observer.OnNext(new Exception("Connection Parameter is invalid"));
				observer.OnCompleted();
				yield break;
			}
			_Param = param;

			m_Socket = new WebSocket(param.EndPoint);

#if DEBUG
			m_Socket.Log.Level = LogLevel.Info;
			m_Socket.Log.File = "ws.log";
#endif

			m_Socket.OnOpen += (sender, e) =>{ OnOpen(e); };
			m_Socket.OnMessage += (sender, e) => { OnMessage(e); };
			m_Socket.OnError += (sender, e) => { OnError(e); };
			m_Socket.OnClose += (sender, e) => { OnClose(e); };

			m_Socket.TcpTimeout = TimeSpan.FromSeconds(5);

			m_Socket.ConnectAsync();

			while(!_IsConnected && ConnectionError == null)
			{
				yield return null;
			}

			observer.OnNext(ConnectionError);
			observer.OnCompleted();
		} 

		public void Send(JObject obj)
		{
			m_Socket.Send(obj.ToString());
		}

		private void OnDestroy()
		{
			if(m_Socket != null && m_Socket.IsAlive)
			{
				m_Socket.Close();
				m_Socket = null;
			}
		}

		#region OnOpen
		private void OnOpen(EventArgs args)
		{
			Debug.Log("WebSocket Open");

			_IsConnected = true;

			_Param.OnOpen();
		}
		#endregion

		#region OnMessage
		private void OnMessage(MessageEventArgs args)
		{
			Debug.Log("WebSocket Message Data: " + args.Data);

			_Param.OnMessage(args.Data);
		}
		#endregion

		#region OnError
		private void OnError(ErrorEventArgs args)
		{
			Debug.Log("WebSocket Error Message: " + args.Message);

			// 未接続の場合、接続時エラーをセットする
			if(!_IsConnected)
			{
				ConnectionError = new Exception(args.Message);
			}

			_Param.OnError(args.Message);
		}
		#endregion

		#region OnClose
		private void OnClose(CloseEventArgs args)
		{
			string message = string.Format("WebSocket Close\n Code : [{0}]\n Reason : [{1}]", args.Code, args.Reason);
			Debug.Log(message);

			if((CloseStatusCode)args.Code != CloseStatusCode.Normal && (CloseStatusCode)args.Code != CloseStatusCode.NoStatus)
			{
				// 未接続の場合、接続時エラーをセットする
				if(!_IsConnected)
				{
					ConnectionError = new Exception(message);
				}
			}

			_IsConnected = false;

			_Param.OnClose();
		}
		#endregion

		private void OnApplicationPause(bool pauseStatus)
		{
			if(pauseStatus)
			{
				Debug.Log("applicationWillResignActive or onPause");
			}
			else
			{
				Debug.Log("applicationDidBecomeActive or onResume");
			}
		}
	}
}