using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	public class PlayerCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform m_Target = null;

		/// <summary> 入力管理クラス </summary>
		[SerializeField]
		private PlayerInputController m_Input = null;

		private Vector3 m_TargetPosition = Vector3.zero;

		private Vector3 m_InputVec = Vector3.zero;

		private float m_RotateSpeed = 50.0f;

		private float m_RollupSpeed = 20.0f;

		private void Start()
		{
			m_TargetPosition = m_Target.transform.position;

			this.UpdateAsObservable().Subscribe(_ =>
			{
				transform.position += (m_Target.transform.position - m_TargetPosition);

				m_TargetPosition = m_Target.transform.position;

				m_InputVec.x = m_Input.GetCameraAxisHorizontal();
				m_InputVec.y = m_Input.GetCameraAxisVertical();

				// 左右の回転
				transform.RotateAround(m_TargetPosition, Vector3.up, m_InputVec.x * (Time.deltaTime * m_RotateSpeed));

				// 上下の回転
				transform.RotateAround(m_TargetPosition, transform.right, m_InputVec.y * (Time.deltaTime * m_RollupSpeed));

			}).AddTo(this);
		}
	}
}