using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public static class SampleDefine
	{
		public static readonly string EndPoint = "ws://localhost:8080";

		public static readonly string KeyUserName = "userName";
		public static readonly string KeyPassword = "password";
	}

	public class SampleResponseRegister : AResponse
	{
		public string UserName { get; private set; }

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			UserName = obj[KeyData][SampleDefine.KeyUserName].ToObject<string>();
		}

		public override string ToString()
		{
			return string.Format("SampleResponseRegister\n ResponseCode={0} UserName={1}", ResponseCode, UserName);
		}
	}

	public class SampleResponseLogin : AResponse
	{
		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();
		}

		public override string ToString()
		{
			return string.Format("SampleResponseLogin\n ResponseCode={0}", ResponseCode);
		}
	}
}