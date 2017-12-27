using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public abstract class AbilityComponent : MonoBehaviour
    {
        public bool IsEnable { get { return m_IsEnable; } }

        protected GameObject m_BaseObject = null;

        protected bool m_IsEnable = false;

        //public abstract void Initialize();

        public abstract void Run();

        public abstract void Stop();
    }
}