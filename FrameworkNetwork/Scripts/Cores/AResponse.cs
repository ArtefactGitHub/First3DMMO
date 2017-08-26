using Newtonsoft.Json.Linq;

namespace com.Artefact.FrameworkNetwork.Cores
{
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