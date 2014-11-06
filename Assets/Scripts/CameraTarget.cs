using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour
{
	Transform m_target = null;
	Vector3 m_offset = Vector3.zero;

	// Use this for initialization
	void Awake()
	{
		renderer.enabled = false;

		m_target = transform.parent;

		if(m_target == null)
			return;

		transform.parent = m_target.parent;
		transform.LookAt(m_target, Vector3.up);

		m_offset = transform.position - m_target.position;
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if(m_target == null)
			return;

		transform.position = m_target.position + m_offset;
	}
}
