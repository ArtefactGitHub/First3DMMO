using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.FirstTutorial
{
	public interface IObjectSpawnController
	{
		void SpawnRandomArea(GameObject parent, GameObject prefab, Vector3 position_1, Vector3 position_2);
	}

	public class ObjectSpawnController : MonoBehaviour, IObjectSpawnController
	{
		#region singleton
		private static ObjectSpawnController m_Instance = null;

		public static IObjectSpawnController Instance
		{
			get
			{
				if(m_Instance == null)
				{
					m_Instance = FindObjectOfType<ObjectSpawnController>();
				}
				return m_Instance;
			}
		}
		#endregion

		public void SpawnRandomArea(GameObject parent, GameObject prefab, Vector3 leftFar, Vector3 rightNear)
		{
			Assert.IsNotNull(prefab);

			float x = UnityEngine.Random.Range(leftFar.x, rightNear.x);
			float z = UnityEngine.Random.Range(leftFar.z, rightNear.z);
			Vector3 pos = new Vector3(x, 0f, z);

			GameObject go = Instantiate(prefab, pos, Quaternion.identity);
			go.transform.SetParent(parent.transform, false);
		}
	}
}