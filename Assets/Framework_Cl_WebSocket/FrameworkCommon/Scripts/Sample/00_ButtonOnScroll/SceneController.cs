using com.Artefact.FrameworkCommon.Cores.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.FrameworkCommon.Sample.ButtonOnScroll
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text = null;

        private void Start()
        {
            // スクロール上のボタンのイベントへ通知しやすくするために、ドラッグの閾値を変更
            EventSystemManager.Instance.SetPixelDragThreshold(15);
        }

        public void OnClick()
        {
            Debug.Log("OnClick");

            m_Text.text = "OnClick : " + Time.frameCount;
        }
    }
}