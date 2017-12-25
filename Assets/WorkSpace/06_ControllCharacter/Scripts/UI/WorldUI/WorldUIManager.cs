using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public class WorldUIManager : MonoBehaviour
	{
        #region singleton
        public static WorldUIManager Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType<WorldUIManager>();
                    Assert.IsNotNull(m_Instance);
                }
                return m_Instance;
            }
        }

        private static WorldUIManager m_Instance = null;
        #endregion

        [SerializeField]
        private TargetMarkerObject m_TargetMarker = null;

        public void Initialize()
        {
            m_TargetMarker.Initialize();
        }

        public void SetTargetMarker(ATargetable target)
        {
            m_TargetMarker.SetTarget(target);
        }
    }
}