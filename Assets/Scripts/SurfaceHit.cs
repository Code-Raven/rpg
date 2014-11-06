using UnityEngine;
using System.Collections;
using Utility;

public class SurfaceHit : MonoBehaviour
{
	public bool m_debug = false;

	Ray m_surfaceRay = new Ray();

	static RaycastHit m_hit = new RaycastHit();

	public static RaycastHit Hit
	{
		get { return m_hit; }
	}
	
	// Update is called once per frame
	void Update()
	{
		m_surfaceRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(m_surfaceRay, out m_hit, Mathf.Infinity, 1 << gameObject.layer);

		if(m_debug && Input.GetMouseButton(0) && m_hit.transform != null)
			Debug.DrawLine(m_hit.point, m_hit.point + (Vector3.up * 10.0f), Color.red);
	}

	public static bool Follow(Transform follower)
	{
		return Follow(follower, -1.0f);
	}

	public static bool Follow(Transform follower, float feetPerSecond)
	{
		return Follow(follower, feetPerSecond, true, false);
	}

	public static bool Follow(Transform follower, float feetPerSecond, bool lookYaw, bool lookPitch)
	{
		if(Hit.transform == null)
			return false;
		
		if(feetPerSecond < 0)
		{
			LookAt(follower, Hit.point, lookYaw, lookPitch);
			follower.position = Hit.point;
			return true;
		}

		float metersPerSecond = Conversion.FeetToMeters(feetPerSecond);
		
		Vector3 lerpedPos = Vector3.Lerp(follower.position, Hit.point,
			metersPerSecond * Time.fixedDeltaTime);
		
		LookAt(follower, Hit.point, lookYaw, lookPitch);
		
		Vector3 travelDir = lerpedPos - follower.position;
		float distTraveled = travelDir.magnitude;
		float maxTravelDist = metersPerSecond * Time.fixedDeltaTime;
		
		if(distTraveled > maxTravelDist)
			lerpedPos = follower.position + (travelDir.normalized * maxTravelDist);

		follower.position = lerpedPos;
		
		return true;
	}

	public static void LookAt(Transform looker, Vector3 lookPosition, bool lookYaw, bool lookPitch)
	{
		Vector3 lookDir = (lookPosition - looker.position).normalized;

		if(!lookYaw)
		{
			lookDir.x = looker.forward.x;
			lookDir.z = looker.forward.z;
		}
		
		if(!lookPitch)
			lookDir.y = looker.forward.y;

		LookAt(looker, lookDir);
	}

	static float m_lookThreshold = 0.1f;

	public static void LookAt(Transform looker, Vector3 lookDir)
	{
		if(lookDir.magnitude > m_lookThreshold)
			looker.forward = lookDir;
	}
}
