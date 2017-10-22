using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.InputStick
{
	public class PlayerInputController : MonoBehaviour
	{
		[SerializeField]
		private bool m_UseStick = true;

		/// <summary> 左スティック </summary>
		[SerializeField]
		private InputStickObject m_LeftStick = null;

		/// <summary> 右スティック </summary>
		[SerializeField]
		private InputStickObject m_RightStick = null;

		public float GetAxisHorizontal()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_LeftStick.Vector.x;
			else result = Input.GetAxis("Horizontal"); 
#else
			result = m_LeftStick.Vector.x;
#endif

			return result;
		}

		public float GetAxisVertical()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_LeftStick.Vector.y;
			else result = Input.GetAxis("Vertical"); 
#else
			result = m_LeftStick.Vector.y;
#endif

			return result;
		}
	}
}