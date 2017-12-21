using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace com.Artefact.FrameworkCommon.Cores.Managers
{
    public class EventSystemManager : MonoBehaviour
    {
        #region singleton
        public static EventSystemManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<EventSystemManager>();
                    Assert.IsNotNull(m_Instance);
                }
                return m_Instance;
            }
        }

        private static EventSystemManager m_Instance = null;
        #endregion

        private EventSystem m_EventSystem = null;

        private void Awake()
        {
            m_EventSystem = FindObjectOfType<EventSystem>();
        }

        public void SetPixelDragThreshold(int threshold)
        {
            if (m_EventSystem != null)
            {
                m_EventSystem.pixelDragThreshold = threshold;
            }
        }
    }
}
