using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.FrameworkNetwork.Cores
{
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

	#region MessageDataHeader

	[JsonObject]
	public class MessageData
	{
		[JsonProperty("commandName")]
		public string CommandName { get; private set; }

		[JsonProperty("exceptionMessage")]
		public string ExceptionMessage { get; private set; }

		[JsonProperty("result")]
		public JObject Result { get; private set; }

		[JsonProperty("isPushMessage")]
		public bool IsPushMessage { get; private set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(CommandName);
		}
	}

	#endregion
}