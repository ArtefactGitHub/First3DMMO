using System;
using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class PlayerModule : AModule
	{
		public override string ModuleName { get { return "player"; } }

		public IObservable<IResponseResult<ResponseOnlyCode>> UpdatePosition(ISyncPlayerData playerData)
		{
			JObject obj = new JObject();

			SyncPlayerData data = new SyncPlayerData();
			data.Setup(playerData.PlayerIndex, playerData.PositionX, playerData.PositionZ);
			obj.Add("data", JToken.FromObject(data));

			return Command<ResponseOnlyCode>(obj, "updatePosition");
		}

		public IObservable<IResponseResult<ResponseOnlyCode>> DisConnection(ISyncPlayerData playerData)
		{
			JObject obj = new JObject();

			SyncPlayerData data = new SyncPlayerData();
			data.Setup(playerData.PlayerIndex, playerData.PositionX, playerData.PositionZ);
			obj.Add("data", JToken.FromObject(data));

			return Command<ResponseOnlyCode>(obj, "disConnection");
		}
	}
}