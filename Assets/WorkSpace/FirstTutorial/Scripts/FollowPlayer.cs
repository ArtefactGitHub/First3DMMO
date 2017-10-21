using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	[SerializeField]
	private Transform m_Target = null;

	private Vector3 m_Offset = Vector3.zero;

	private void Start()
	{
		m_Offset = GetComponent<Transform>().position - m_Target.position;
	}

	private void Update()
	{
		GetComponent<Transform>().position = m_Target.position + m_Offset;
	}
}
