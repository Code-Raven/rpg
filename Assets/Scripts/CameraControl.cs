using UnityEngine;
using System.Collections;
using Utility;

public class CameraControl : MonoBehaviour
{
	public CameraTarget m_cameraTarget = null;
	public float m_lerpSpeed = 10.0f;

	public static event VoidEventHandler RenderTarget = null;

	void FixedUpdate()
	{
		if(m_cameraTarget == null)
			return;

		transform.position = Vector3.Lerp(transform.position, 
			m_cameraTarget.transform.position, m_lerpSpeed * Time.deltaTime);
		transform.rotation = m_cameraTarget.transform.rotation;
	}

	void OnPostRender()
	{        
		if(RenderTarget != null)
			RenderTarget();
	}
}
