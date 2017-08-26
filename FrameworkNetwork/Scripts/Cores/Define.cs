using System;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public class Response
	{
		public int ResponseCode { get; private set; }
	}

	#region ConnectionParameter
	public class ConnectionParameter
	{
		public string EndPoint { get; set; }

		public Action OnOpen { get; set; }

		public Action<string> OnMessage { get; set; }

		public Action<string> OnError { get; set; }

		public Action OnClose { get; set; }

		public ConnectionParameter(string endPoint, Action onOpen, Action<string> onMessage, Action<string> onError, Action onClose)
		{
			this.EndPoint = endPoint;
			this.OnOpen = onOpen;
			this.OnMessage = onMessage;
			this.OnError = onError;
			this.OnClose = onClose;
		}

		public bool IsValid()
		{
			return (
				!string.IsNullOrEmpty(EndPoint) &&
				OnOpen != null &&
				OnMessage != null &&
				OnError != null &&
				OnClose != null);
		}
	}
	#endregion
}