using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private float m_Speed = 10f;

		/// <summary> 入力管理クラス </summary>
		[SerializeField]
		private PlayerInputController m_Input = null;

		private Vector3 m_InputVec = Vector3.zero;

		private Vector3 m_CalcVec = Vector3.zero;

		private Rigidbody m_Rigidbody = null;

		void Start()
		{
			// 回転しないようにする
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

			// 左スティック入力
			m_Input.OnInputLeftStickAsObservable.Subscribe(inputVec =>
			{
				m_InputVec.x = inputVec.x;
				m_InputVec.z = inputVec.y;
			}).AddTo(this);

			// FixedUpdate()
			this.FixedUpdateAsObservable().Subscribe(_ =>
			{
				// カメラの進行方向ベクトル
				m_CalcVec.Set(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
				Vector3 cameraForward = m_CalcVec.normalized;

				// カメラと前方ベクトルとの角度を求める
				// 前方ベクトルを向いている場合 0 となる（-180.0f <= 0 <= 180.0f の範囲）
				float angle = Vector3.Angle(Vector3.forward, cameraForward) * (cameraForward.x < 0f ? -1.0f : 1f);

				// 角度分、入力ベクトルを回転させる
				var moveVec = Quaternion.AngleAxis(angle, Vector3.up) * m_InputVec;

				m_Rigidbody.velocity = moveVec * m_Speed;

				if(moveVec != Vector3.zero)
				{
					transform.rotation = Quaternion.LookRotation(moveVec);
				}
			}).AddTo(this);
		}
	}
}