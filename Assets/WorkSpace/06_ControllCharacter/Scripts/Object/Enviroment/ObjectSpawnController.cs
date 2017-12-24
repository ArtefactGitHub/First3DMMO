using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public interface IObjectSpawnController
	{
        List<ATargetable> Targetables { get; }

        void SpawnRandomArea(GameObject parent, GameObject prefab, Vector3 position_1, Vector3 position_2);

        void Spawn(GameObject parent, GameObject prefab, Vector3 position);
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

        public List<ATargetable> Targetables { get; private set; }

        [SerializeField]
        private string m_NameFormat = "Cyl_{0:D4}";

        void Awake()
        {
            Targetables = new List<ATargetable>();
        }

        public void Spawn(GameObject parent, GameObject prefab, Vector3 position)
        {
            GameObject go = Instantiate(prefab, position, Quaternion.identity);
            go.name = string.Format(m_NameFormat, Targetables.Count);

            go.transform.SetParent(parent.transform, false);

            var targetable = go.GetComponent<ATargetable>();
            if (targetable != null)
            {
                Targetables.Add(targetable);
            }
        }

        public void SpawnRandomArea(GameObject parent, GameObject prefab, Vector3 leftFar, Vector3 rightNear)
		{
			Assert.IsNotNull(prefab);

			float x = Random.Range(leftFar.x, rightNear.x);
			float z = Random.Range(leftFar.z, rightNear.z);
			Vector3 pos = new Vector3(x, 0f, z);

			GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            go.name = string.Format(m_NameFormat, Targetables.Count);

            go.transform.SetParent(parent.transform, false);

            var targetable = go.GetComponent<ATargetable>();
            if(targetable != null)
            {
                Targetables.Add(targetable);
            }
		}
	}
}