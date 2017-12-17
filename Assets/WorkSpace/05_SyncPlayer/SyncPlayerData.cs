using System;
using Newtonsoft.Json;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public interface ISyncPlayerData
	{
		int PlayerIndex { get; }

		float PositionX { get; }

		float PositionZ { get; }
	}

	[JsonObject]
	public class SyncPlayerData : ISyncPlayerData
	{
		[JsonIgnore]
		public int PlayerIndex { get { return playerIndex; } }

		[JsonIgnore]
		public float PositionX { get { return positionX; } }

		[JsonIgnore]
		public float PositionZ { get { return positionZ; } }

		[JsonProperty]
		private int playerIndex { get; set; }

		[JsonProperty]
		private float positionX { get; set; }

		[JsonProperty]
		private float positionZ { get; set; }

		public void Setup(int index, float x, float z)
		{
			this.playerIndex = index;
			this.positionX = x;
			this.positionZ = z;
		}

		public override string ToString()
		{
			return string.Format("idx=[{0}] [{1}, {2}]", PlayerIndex, PositionX, PositionZ);
		}
	}
}