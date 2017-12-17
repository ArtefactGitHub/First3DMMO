using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class ResponseSyncPlayer : AResponse
	{
		public ISyncPlayerData SyncPlayerData
		{
			get
			{
				return m_SyncPlayerData;
			}
		}

		private SyncPlayerData m_SyncPlayerData = new SyncPlayerData();

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			int playerIndex = obj[KeyData]["playerData"]["playerIndex"].ToObject<int>();
			float positionX = obj[KeyData]["playerData"]["positionX"].ToObject<float>();
			float positionZ = obj[KeyData]["playerData"]["positionZ"].ToObject<float>();

			m_SyncPlayerData = new SyncPlayerData();
			m_SyncPlayerData.Setup(playerIndex, positionX, positionZ);
		}

		public override string ToString()
		{
			return (m_SyncPlayerData != null ?
				string.Format("ResponseSyncPlayer\n ResponseCode={0} PlayerIndex={1}", ResponseCode, m_SyncPlayerData.PlayerIndex) :
				string.Format("ResponseSyncPlayer\n ResponseCode={0}", ResponseCode));
		}
	}

	public class ResponseSyncOtherPlayers : AResponse
	{
		public int PlayerIndex { get; private set; }

		public List<ISyncPlayerData> SyncPlayerDatas
		{
			get
			{
				return m_SyncPlayerDatas.Select(x => (ISyncPlayerData)x).ToList();
			}
		}

		private List<SyncPlayerData> m_SyncPlayerDatas = new List<SyncPlayerData>();

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			PlayerIndex = obj[KeyData]["playerIndex"].ToObject<int>();
			m_SyncPlayerDatas = obj[KeyData]["playerDataList"].ToObject<List<SyncPlayerData>>();
		}

		public override string ToString()
		{
			return string.Format("ResponseSyncPlayer\n ResponseCode={0} Count={1}", ResponseCode, m_SyncPlayerDatas.Count);
		}
	}

	public class ResponseUpdateOtherPlayers : AResponse
	{
		public List<ISyncPlayerData> SyncPlayerDatas
		{
			get
			{
				return m_SyncPlayerDatas.Select(x => (ISyncPlayerData)x).ToList();
			}
		}

		private List<SyncPlayerData> m_SyncPlayerDatas = new List<SyncPlayerData>();

		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();

			m_SyncPlayerDatas = obj[KeyData]["playerDataList"].ToObject<List<SyncPlayerData>>();
		}

		public override string ToString()
		{
			return string.Format("ResponseUpdateOtherPlayers\n ResponseCode={0} Count={1}", ResponseCode, m_SyncPlayerDatas.Count);
		}
	}

	public class ResponseOnlyCode : AResponse
	{
		public override void TryParse(JObject obj)
		{
			ResponseCode = obj[KeyResponseCode].ToObject<int>();
		}

		public override string ToString()
		{
			return string.Format("ResponseOnlyCode\n ResponseCode={0}", ResponseCode);
		}
	}
}