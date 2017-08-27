using Newtonsoft.Json.Linq;
using System;
using UniRx;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public abstract class AModule
	{
		public IObservable<ResponseResult<T>> Command<T>(JObject obj, string commandName) where T : AResponse, new()
		{
			return Connection.Instance.Send(obj, commandName).SelectMany(messageData =>
			{
				if(messageData == null)
				{
					return Observable.Return(new ResponseResult<T>(new Exception("no message data"), null));
				}

				if(!string.IsNullOrEmpty(messageData.ExceptionMessage))
				{
					return Observable.Return(new ResponseResult<T>(new Exception(messageData.ExceptionMessage), null));
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

				return Observable.Return(new ResponseResult<T>(ex, response));
			});
		}
	}
}