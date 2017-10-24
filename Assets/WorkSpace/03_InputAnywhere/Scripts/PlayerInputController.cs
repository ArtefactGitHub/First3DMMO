using UniRx;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	public class PlayerInputController : MonoBehaviour
	{
		/// <summary> スティック入力 </summary>
		[SerializeField]
		private InputStickObject m_InputStick = null;

		/// <summary>
		/// 左スティック入力のストリーム
		/// </summary>
		public IObservable<Vector2> OnInputLeftStickAsObservable { get { return m_InputStick.OnInputLeftStickAsObservable; } }

		/// <summary>
		/// 右（カメラ）スティック入力のストリーム
		/// </summary>
		public IObservable<Vector2> OnInputRightStickAsObservable { get { return m_InputStick.OnInputRightStickAsObservable; } }
	}
}