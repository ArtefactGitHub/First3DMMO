using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	public class PlayerInputController : MonoBehaviour
	{
		public bool IsBoost { get; private set; }

		[SerializeField]
		private bool m_UseStick = true;

		/// <summary> 左スティック </summary>
		[SerializeField]
		private InputStickObject m_InputStick = null;

		///// <summary> 右スティック </summary>
		//[SerializeField]
		//private InputStickObject m_RightStick = null;

		[SerializeField]
		private Image  m_Boost = null;

		private void Start()
		{
			// ブースト状態の切り替え
			var trigger = m_Boost.gameObject.AddComponent<ObservableEventTrigger>();
			trigger.OnPointerDownAsObservable().Subscribe(_ => IsBoost = true).AddTo(this);
			trigger.OnPointerUpAsObservable().Subscribe(_ => IsBoost = false).AddTo(this);
		}

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