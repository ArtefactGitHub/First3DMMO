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

		public Action<IMessageData> OnMessage { get; set; }

		public Action<string> OnError { get; set; }

		public Action OnClose { get; set; }

		public ConnectionParameter(string endPoint, Action onOpen, Action<IMessageData> onMessage, Action<string> onError, Action onClose)
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

	public interface IMessageData
	{
		string CommandName { get; }

		string ExceptionMessage { get; }

		JObject Result { get; }

		bool IsPushMessage { get; }
	}

	[JsonObject]
	public class MessageData : IMessageData
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

		public override string ToString()
		{
			return string.Format("CommandName=[{0}] ExceptionMessage=[{1}] Result=[{2}] IsPushMessage=[{3}]",
				CommandName, ExceptionMessage, Result, IsPushMessage);
		}
	}

	public class MessageDataException : IMessageData
	{
		public string CommandName { get; private set; }

		public string ExceptionMessage { get; private set; }

		public JObject Result { get; private set; }

		public bool IsPushMessage { get; private set; }

		public MessageDataException(string exceptionMessage)
		{
			this.ExceptionMessage = exceptionMessage;
		}
	}

	#endregion
}