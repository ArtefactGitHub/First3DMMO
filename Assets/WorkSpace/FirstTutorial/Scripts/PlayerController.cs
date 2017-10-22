using com.Artefact.First3DMMO.WorkSpace.InputStick;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.FirstTutorial
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private float Movement = 500f;

		[SerializeField]
		private float RotateSpeed = 10f;

		/// <summary> 入力管理クラス </summary>
		[SerializeField]
		private PlayerInputController m_Input = null;

		private Vector3 m_VecMove = Vector3.zero;

		private Rigidbody m_Rigidbody;

		void Start()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			// 回転しないようにする
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}

		void Update()
		{
			m_VecMove.x = m_Input.GetAxisHorizontal() * Time.deltaTime * Movement;
			m_VecMove.z = m_Input.GetAxisVertical() * Time.deltaTime * Movement;

			if(m_VecMove.magnitude > 0.01f)
			{
				float rotateDelta = RotateSpeed * Time.deltaTime;
				Quaternion quaterninon = Quaternion.LookRotation(m_VecMove);
				transform.rotation = Quaternion.Lerp(transform.rotation, quaterninon, rotateDelta);
			}
		}

		void FixedUpdate()
		{
			m_Rigidbody.velocity = m_VecMove;
		}
	}
}