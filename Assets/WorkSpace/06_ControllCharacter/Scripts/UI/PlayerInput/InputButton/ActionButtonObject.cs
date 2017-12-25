using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class ActionButtonObject : MonoBehaviour
    {
        public IObservable<Unit> OnClickAsObservable { get { return m_ActionButton.OnClickAsObservable(); } }

        [SerializeField]
        private Button m_ActionButton = null;
    }
}