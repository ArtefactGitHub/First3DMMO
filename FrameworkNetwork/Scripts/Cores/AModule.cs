using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public class AModule
	{
		public void Command<T>(JObject obj, string commandName, Action<Exception, T> callback) where T : new()
		{
			Connection.Instance.Send(obj);
		}
	}
}