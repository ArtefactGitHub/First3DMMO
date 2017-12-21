using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public class PlayerCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform m_Target = null;

		[SerializeField]
		private float m_RotateSpeed = 50.0f;

		[SerializeField]
		private float m_RollupSpeed = 20.0f;

        /// <summary> 入力管理クラス </summary>
        private PlayerInputStickManager m_Input = null;

        private Vector3 m_TargetPosition = Vector3.zero;

		private Vector3 m_InputVec = Vector3.zero;

		private void Start()
        {
            // 操作管理クラスのインスタンスの取得
            m_Input = PlayerInputStickManager.Instance;
            Assert.IsNotNull(m_Input);

            m_TargetPosition = m_Target.transform.position;

			// 右（カメラ）スティック入力
			m_Input.OnInputRightStickAsObservable.Subscribe(inputVec =>
			{
				m_InputVec = inputVec;
			}).AddTo(this);

			// Update()
			this.UpdateAsObservable().Subscribe(_ =>
			{
				transform.position += (m_Target.transform.position - m_TargetPosition);

				m_TargetPosition = m_Target.transform.position;

				// 左右の回転
				transform.RotateAround(m_TargetPosition, Vector3.up, m_InputVec.x * (Time.deltaTime * m_RotateSpeed));

				// 上下の回転
				transform.RotateAround(m_TargetPosition, transform.right, m_InputVec.y * (Time.deltaTime * m_RollupSpeed));
			}).AddTo(this);
		}
	}
}