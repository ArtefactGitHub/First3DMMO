using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float m_Speed = 10.0f;

	private void FixedUpdate()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Rigidbody rigidbody = GetComponent<Rigidbody>();

		rigidbody.AddForce(x * m_Speed, 0, z * m_Speed);
	}
}
