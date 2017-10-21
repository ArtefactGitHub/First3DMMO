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
			m_VecMove.x = Input.GetAxis("Horizontal") * Time.deltaTime * Movement;
			m_VecMove.z = Input.GetAxis("Vertical") * Time.deltaTime * Movement;

			//Debug.Log(string.Format("H[{0}] V[{1}]", Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
			//Debug.Log(string.Format("x[{0}] z[{1}]", m_VecMove.x, m_VecMove.z));

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