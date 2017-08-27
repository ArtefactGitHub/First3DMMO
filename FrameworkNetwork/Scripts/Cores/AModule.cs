using Newtonsoft.Json.Linq;
using System;
using UniRx;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public abstract class AModule
	{
		public abstract string ModuleName { get; }

		public IObservable<IResponseResult<T>> Command<T>(JObject obj, string commandName) where T : AResponse, new()
		{
			// ヘッダー情報を追加
			obj.Add("state", AddDataHeader(commandName));

			return Connection.Instance.Send(obj, commandName).SelectMany(messageData =>
			{
				ResponseResult<T> result = new ResponseResult<T>();

				if(messageData == null)
				{
					result.SetParameter(new Exception("no message data"), null);
					return Observable.Return<IResponseResult<T>>(result);
				}

				if(!string.IsNullOrEmpty(messageData.ExceptionMessage))
				{
					result.SetParameter(new Exception(messageData.ExceptionMessage), null);
					return Observable.Return<IResponseResult<T>>(result);
				}

				T response = new T();
				Exception ex = null;
				try
				{
					response.TryParse(messageData.Result);
				}
				catch(Exception exParse)
				{
					ex = exParse;
					throw;
				}

				result.SetParameter(ex, response);
				return Observable.Return<IResponseResult<T>>(result);
			});
		}

		private JObject AddDataHeader(string commandName)
		{
			JObject state = new JObject();
			state.Add("id", new JValue(0));
			state.Add("name", new JValue(""));
			state.Add("module", new JValue(ModuleName));
			state.Add("command", new JValue(commandName));

			return state;
		}
	}
}