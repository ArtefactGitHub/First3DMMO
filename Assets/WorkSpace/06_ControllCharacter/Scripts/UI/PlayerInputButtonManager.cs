using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public interface IPlayerInputButtonManager
    {
        IObservable<bool> IsTargetLockAsObservable { get; }

        void Initialize();

        void SetEnableTargetLock(bool isEnable);
    }

    public class PlayerInputButtonManager : MonoBehaviour, IPlayerInputButtonManager
    {
        #region singleton
        public static IPlayerInputButtonManager Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType<PlayerInputButtonManager>();
                    Assert.IsNotNull(m_Instance);
                }
                return m_Instance;
            }
        }

        private static PlayerInputButtonManager m_Instance = null;
        #endregion

        public IObservable<bool> IsTargetLockAsObservable { get { return m_ToggleTargetLockObject.IsLockAsObservable; } }

        [SerializeField]
        private ToggleTargetLockObject m_ToggleTargetLockObject = null;

        public void Initialize()
        {
            m_ToggleTargetLockObject.Initialize();
        }

        public void SetEnableTargetLock(bool isEnable)
        {
            m_ToggleTargetLockObject.SetEnable(isEnable);
        }
    }
}