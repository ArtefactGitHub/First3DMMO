using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using WebSocketSharp;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public interface IConnection
	{
		/// <summary>
		/// 初期化処理
		/// 
		/// 接続先やメッセージイベントの Action を登録した
		/// パラメータークラスを引数として渡してください。
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		IObservable<Exception> Initialize(ConnectionParameter param);

		/// <summary>
		/// ユーザーコマンドの送信
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="commandName"></param>
		/// <param name="callback"></param>
		IObservable<IMessageData> Send(JObject obj, string commandName);
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

		/// <summary> 接続パラメータ </summary>
		private ConnectionParameter _ConnectionParam = null;

		/// <summary> 接続されたか </summary>
		private bool _IsConnected { get; set; }

		/// <summary> 接続時エラー </summary>
		private Exception ConnectionError { get; set; }

		/// <summary>
		/// OnMessage で受け取ったメッセージデータ 
		/// 
		/// MainThread で処理する必要があるため、一度キャッシュしておき、
		/// Update() 内で処理します。
		/// </summary>
		private List<MessageData> _MessageDataQueues = new List<MessageData>();

		#region Initialize

		public IObservable<Exception> Initialize(ConnectionParameter param)
		{
			return Observable.FromCoroutine<Exception>(observer => ConnectProcess(observer, param)).SelectMany(ex =>
			{
				return Observable.Return(ex);
			});
		}

		public IEnumerator ConnectProcess(IObserver<Exception> observer, ConnectionParameter param)
		{
			if(param == null || !param.IsValid())
			{
				observer.OnNext(new Exception("Connection Parameter is invalid"));
				observer.OnCompleted();
				yield break;
			}
			_ConnectionParam = param;

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

		#endregion

		#region Send

		public IObservable<IMessageData> Send(JObject obj, string commandName)
		{
			m_Socket.Send(obj.ToString());

			// ユーザーコマンドに対する受信を非同期で待ち、IMessageData の形で返す
			return Observable.FromCoroutine<IMessageData>(observer => GetReceivedMessageData(observer, commandName)).SelectMany(messageData =>
			{
				return Observable.Return(messageData);
			});
		}

		/// <summary>
		/// ユーザーコマンドの受信データを取得する
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="commandName"></param>
		/// <returns></returns>
		private IEnumerator GetReceivedMessageData(IObserver<IMessageData> observer, string commandName)
		{
			while(true)
			{
				MessageData messageData = _MessageDataQueues.FirstOrDefault(x => x.CommandName.Equals(commandName));
				if(messageData != null && messageData.IsValid())
				{
					observer.OnNext(messageData);
					observer.OnCompleted();
					yield break;
				}

				yield return null;
			}
		}

		#endregion

		private void Update()
		{
			// サーバーからプッシュされたメッセージ（ユーザーコマンドの戻り値ではない）を
			// 受信していた場合、直接アプリケーション側へ渡す
			if(_MessageDataQueues.Count > 0)
			{
				MessageData messageData = GetPushMessageData();
				if(messageData != null)
				{
					_ConnectionParam.OnMessage(messageData.Result.ToString());

					_MessageDataQueues.Remove(messageData);
				}
			}
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

			_ConnectionParam.OnOpen();
		}
		#endregion

		#region OnMessage

		/// <summary>
		/// サーバーからのデータ受信イベント
		/// 
		/// MainThread で処理する必要があるため、一度キャッシュしておき、
		/// Update() 内で処理します。
		/// </summary>
		/// <param name="args"></param>
		private void OnMessage(MessageEventArgs args)
		{
			if(!string.IsNullOrEmpty(args.Data))
			{
				MessageData messageData = ParseMessageData(args.Data);
				if(messageData != null && messageData.IsValid())
				{
					_MessageDataQueues.Add(messageData);
				}
			}
		}

		private MessageData ParseMessageData(string messageData)
		{
			MessageData result = null;
			try
			{
				if(!string.IsNullOrEmpty(messageData))
				{
					Debug.Log("ParseMessageData : " + messageData);

					JObject obj = JObject.Parse(messageData);
					result = obj.ToObject<MessageData>();
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
				throw;
			}

			return result;
		}

		/// <summary>
		/// サーバーからプッシュされたメッセージの取得
		/// </summary>
		/// <returns></returns>
		private MessageData GetPushMessageData()
		{
			MessageData result = _MessageDataQueues.FirstOrDefault(messageData =>
			{
				if(messageData != null && messageData.IsValid())
				{
					if(messageData.IsPushMessage)
					{
						return true;
					}
				}

				return false;
			});

			return result;
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

			_ConnectionParam.OnError(args.Message);
		}

		#endregion

		#region OnClose

		private void OnClose(CloseEventArgs args)
		{
			string message = string.Format("WebSocket Close\n Code : [{0}]\n Reason : [{1}]", args.Code, args.Reason);
			Debug.Log(message);

			// http://tools.ietf.org/html/rfc6455#section-7.4
			if((CloseStatusCode)args.Code != CloseStatusCode.Normal && (CloseStatusCode)args.Code != CloseStatusCode.NoStatus)
			{
				// 未接続の場合、接続時エラーをセットする
				if(!_IsConnected)
				{
					ConnectionError = new Exception(message);
				}
			}

			_IsConnected = false;

			_ConnectionParam.OnClose();
		}

		#endregion

		private void OnApplicationPause(bool pauseStatus)
		{
			//if(pauseStatus)
			//{
			//	Debug.Log("applicationWillResignActive or onPause");
			//}
			//else
			//{
			//	Debug.Log("applicationDidBecomeActive or onResume");
			//}
		}
	}
}