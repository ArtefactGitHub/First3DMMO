using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	public class PlayerInputController: MonoBehaviour
	{
        #region singleton
        public static PlayerInputController Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType<PlayerInputController>();
                    Assert.IsNotNull(m_Instance);
                }
                return m_Instance;
            }
        }

        private static PlayerInputController m_Instance = null;
        #endregion

        public PlayerInputStickManager InputStickManager { get { return m_InputStickManager; } }

        /// <summary> スティック入力 </summary>
        [SerializeField]
		private PlayerInputStickManager m_InputStickManager = null;

        public PlayerInputButtonManager InputButtonManager { get { return m_InputButtonManager; } }

        /// <summary> ボタン入力 </summary>
        [SerializeField]
        private PlayerInputButtonManager m_InputButtonManager = null;

        public void Initialize()
        {
            InputStickManager.Initialize();

            InputButtonManager.Initialize();
        }
    }
}