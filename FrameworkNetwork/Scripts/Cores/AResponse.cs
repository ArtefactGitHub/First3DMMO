using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.FrameworkNetwork.Cores
{
	public class ResponseResult<T> where T : AResponse, new()
	{
		public Exception Exception { get; private set; }

		public T Result { get; private set; }

		public ResponseResult(Exception ex, T result)
		{
			this.Exception = ex;
			this.Result = result;
		}
	}

	/// <summary>
	/// サーバーからの受信データクラスの抽象クラス
	/// 
	/// 全ての通信で、このクラスを継承したクラスを戻り値として指定してください。
	/// </summary>
	public abstract class AResponse
	{
		public static readonly string KeyResponseCode = "responceCode";

		public static readonly string KeyData = "data";

		public abstract void TryParse(JObject obj);

		public int ResponseCode { get; protected set; }
	}
}