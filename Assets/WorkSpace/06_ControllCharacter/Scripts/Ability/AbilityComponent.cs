using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public abstract class AbilityComponent : MonoBehaviour
    {
        protected GameObject m_BaseObject = null;

        protected bool m_IsEnable = false;

        //public abstract void Initialize();

        public abstract void Run();

        public abstract void Stop();
    }
}