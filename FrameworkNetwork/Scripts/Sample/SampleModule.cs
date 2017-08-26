using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public class SampleModule : AModule
	{
		public void Register(string userName, Action<Response> callback)
		{
			string commandName = "register";

			JObject state = new JObject();
			state.Add("id", new JValue(0));
			state.Add("name", new JValue(""));
			state.Add("command", new JValue(commandName));

			JObject data = new JObject();
			data.Add("name", new JValue(userName));

			JObject obj = new JObject();
			obj.Add("state", state);
			obj.Add("data", data);

			Command<Response>(obj, commandName, callback);
		}
	}
}