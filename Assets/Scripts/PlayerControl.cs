using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class PlayerControl : MonoBehaviour
{
	Player m_player = null;

	float m_pathUpdateTime = 0.2f;
	bool m_pathFinding = false;

	// Use this for initialization
	void Awake()
	{
		m_player = GetComponent<Player>();
	}

	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			Move();
			return;
		}

		Stop();
	}

	void Stop()
	{
		if(Input.GetMouseButtonUp(0))
			StopCoroutine("StartPathFinding");

		if(m_pathFinding && !Input.GetMouseButton(0))
		{
			if(!m_player.PathBlocked(SurfaceHit.Hit.point))
			{
				m_player.FindTarget(SurfaceHit.Hit.point);
				m_pathFinding = false;
				StopCoroutine("StartPathFinding");
			}
		}
	}

	void Move()
	{
		if(m_player.PathBlocked(SurfaceHit.Hit.point))
		{
			if(!m_pathFinding)
			{//Debug.LogWarning("Start Path Finding");
				StartCoroutine("StartPathFinding");
				m_pathFinding = true;
			}
			
			return;
		}
		
		if(m_pathFinding)
		{
			StopCoroutine("StartPathFinding");
			m_pathFinding = false;
			return;
		}
		
		if(SurfaceHit.Hit.transform != null)
			m_player.FindTarget(SurfaceHit.Hit.point);
	}

	IEnumerator StartPathFinding()
	{
		while(true)
		{
			if(SurfaceHit.Hit.transform != null)
				m_player.FindPath(SurfaceHit.Hit.point);

			yield return new WaitForSeconds(m_pathUpdateTime);
		}
	}
}
