using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public enum ActionButtonType
    {
        Button_1, Button_2,
    }

    public interface IPlayerInputButtonManager
    {
        IObservable<bool> IsTargetLockAsObservable { get; }

        IObservable<ActionButtonType> OnClickActionButton { get; }

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

        public IObservable<ActionButtonType> OnClickActionButton { get { return m_OnClickActionButton.AsObservable(); } }

        private Subject<ActionButtonType> m_OnClickActionButton = new Subject<ActionButtonType>();

        [SerializeField]
        private ToggleTargetLockObject m_ToggleTargetLockObject = null;

        [SerializeField]
        private ActionButtonObject m_ActionButton_1 = null;

        [SerializeField]
        private ActionButtonObject m_ActionButton_2 = null;

        public void Initialize()
        {
            m_ToggleTargetLockObject.Initialize();

            m_ActionButton_1.OnClickAsObservable.Subscribe(_ => m_OnClickActionButton.OnNext(ActionButtonType.Button_1)).AddTo(this);
            m_ActionButton_2.OnClickAsObservable.Subscribe(_ => m_OnClickActionButton.OnNext(ActionButtonType.Button_2)).AddTo(this);
        }

        public void SetEnableTargetLock(bool isEnable)
        {
            m_ToggleTargetLockObject.SetEnable(isEnable);
        }
    }
}