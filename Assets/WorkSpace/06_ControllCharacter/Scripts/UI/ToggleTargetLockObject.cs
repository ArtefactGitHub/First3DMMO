using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class ToggleTargetLockObject : MonoBehaviour
    {
        public IObservable<bool> IsLockAsObservable { get { return m_IsLockAsObservable.AsObservable(); } }

        private Subject<bool> m_IsLockAsObservable = new Subject<bool>();

        public bool IsLock { get; private set; }

        [SerializeField]
        private Toggle m_Toggle = null;

        public void Initialize()
        {
            m_Toggle.OnValueChangedAsObservable().Subscribe(isOn =>
            {
                IsLock = isOn;
            }).AddTo(this);

            this.ObserveEveryValueChanged(x => x.IsLock).Subscribe(isLock =>
            {
                m_IsLockAsObservable.OnNext(isLock);
            }).AddTo(this);
        }

        public void SetEnable(bool isEnable)
        {
            m_Toggle.interactable = isEnable;

            if (!isEnable)
            {
                m_Toggle.isOn = false;
            }
        }
	}
}