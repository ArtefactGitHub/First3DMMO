using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class ResponseSyncPlayer : AResponse
	{
		public int PlayerIndex { get; private set; }

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			PlayerIndex = obj[KeyData]["playerData"]["playerIndex"].ToObject<int>();
		}

		public override string ToString()
		{
			return string.Format("ResponseSyncPlayer\n ResponseCode={0} PlayerIndex={1}", ResponseCode, PlayerIndex);
		}
	}
}