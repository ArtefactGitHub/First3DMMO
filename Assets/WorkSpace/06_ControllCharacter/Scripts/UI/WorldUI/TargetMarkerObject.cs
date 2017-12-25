using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// </summary>
    /// <summary>
    public class TargetMarkerObject : MonoBehaviour
    {
        [SerializeField]
        private Image m_Image = null;

        private ATargetable m_TargetObj { get; set; }

        private RectTransform m_Rect { get; set; }

        void Start()
        {
            m_Rect = GetComponent<RectTransform>();

            this.UpdateAsObservable().Subscribe(_ =>
            {
                if (m_TargetObj != null)
                {
                    m_Rect.position = RectTransformUtility.WorldToScreenPoint(Camera.main, m_TargetObj.Position);
                }
            }).AddTo(this);
        }

        public void Initialize()
        {
            SetTarget(null);
        }

        public void SetTarget(ATargetable target)
        {
            m_TargetObj = target;

            m_Image.enabled = (m_TargetObj != null);
        }
    }
}