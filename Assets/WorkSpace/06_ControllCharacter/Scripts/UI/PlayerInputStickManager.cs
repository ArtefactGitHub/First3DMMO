using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	public class PlayerInputStickManager : MonoBehaviour
	{
        #region singleton
        public static PlayerInputStickManager Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType<PlayerInputStickManager>();
                    Assert.IsNotNull(m_Instance);
                }
                return m_Instance;
            }
        }

        private static PlayerInputStickManager m_Instance = null;
        #endregion

        /// <summary> スティック入力 </summary>
        [SerializeField]
		private AInputStickObject m_InputStickForMove = null;

        /// <summary> スティック入力 </summary>
        [SerializeField]
        private AInputStickObject m_InputStickForCamera = null;

        /// <summary>
        /// 左スティック入力のストリーム
        /// </summary>
        public IObservable<Vector2> OnInputLeftStickAsObservable { get { return m_InputStickForMove.VectorAsObservable; } }

		/// <summary>
		/// 右（カメラ）スティック入力のストリーム
		/// </summary>
		public IObservable<Vector2> OnInputRightStickAsObservable { get { return m_InputStickForCamera.VectorAsObservable; } }
	}
}