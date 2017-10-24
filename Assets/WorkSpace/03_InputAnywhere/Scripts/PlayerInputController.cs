using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	public class PlayerInputController : MonoBehaviour
	{
		[SerializeField]
		private bool m_UseStick = true;

		/// <summary> 左スティック </summary>
		[SerializeField]
		private InputStickObject m_InputStick = null;

		public float GetAxisHorizontal()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_InputStick.Vector.x;
			else result = Input.GetAxis("Horizontal"); 
#else
			result = m_InputStick.Vector.x;
#endif

			return result;
		}

		public float GetAxisVertical()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_InputStick.Vector.y;
			else result = Input.GetAxis("Vertical"); 
#else
			result = m_InputStick.Vector.y;
#endif

			return result;
		}

		public float GetCameraAxisHorizontal()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_InputStick.CameraVector.x;
			else result = Input.GetAxis("Mouse X");
#else
			result = m_InputStick.CameraVector.x;
#endif

			return result;
		}

		public float GetCameraAxisVertical()
		{
			float result = 0f;

#if UNITY_EDITOR
			if(m_UseStick) result = m_InputStick.CameraVector.y;
			else result = Input.GetAxis("Mouse Y");
#else
			result = m_InputStick.CameraVector.y;
#endif

			return result;
		}
	}
}