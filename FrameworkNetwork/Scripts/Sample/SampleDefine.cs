using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public static class SampleDefine
	{
		public static readonly string EndPoint = "ws://my-nodejs-app-artefactcloud.c9users.io/";
	}

	public class SampleResponseRegister : AResponse
	{
		public string UserName { get; private set; }

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			UserName = obj[KeyData]["userName"].ToObject<string>();
		}

		public override string ToString()
		{
			return string.Format("Response\n ResponseCode={0} UserName={1}", ResponseCode, UserName);
		}
	}
}