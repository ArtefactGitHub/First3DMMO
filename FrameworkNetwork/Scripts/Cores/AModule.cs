using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public abstract class AModule
	{
		public void Command<T>(JObject obj, string commandName, Action<Exception, T> callback) where T : AResponse, new()
		{
#if true
			Connection.Instance.Send(obj, commandName, (Exception ex, JObject result) =>
			{
				if(callback == null)
				{
					return;
				}

				if(ex != null)
				{
					callback.Invoke(ex, default(T));
					return;
				}

				T response = new T();
				try
				{
					response.TryParse(result);
				}
				catch(Exception exParse)
				{
					ex = exParse;
					throw;
				}

				callback.Invoke(ex, response);
			});
#else
			Connection.Instance.Send(obj, commandName, null);
#endif
		}
	}
}