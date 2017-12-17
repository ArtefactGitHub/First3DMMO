using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class PlayerObject : MonoBehaviour
	{
		public ISyncPlayerData PlayerData { get; private set; }

		public void Initialize(ISyncPlayerData playerData)
		{
			UpdateData(playerData);
		}

		public void UpdateData(ISyncPlayerData playerData)
		{
			this.PlayerData = playerData;

			transform.position = new Vector3(PlayerData.PositionX, 0.5f, PlayerData.PositionZ);
		}
	}
}