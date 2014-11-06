using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class AIControl : MonoBehaviour
{
	public float m_attackRange = 5.0f;

	AI m_ai = null;
	Player m_player = null;
	
	float m_pathUpdateTime = 0.2f;
	bool m_pathFinding = false;
	
	// Use this for initialization
	void Awake()
	{
		m_ai = GetComponent<AI>();
	}
	
	void Update()
	{
		Move();
		
		//Stop();
	}
	
	void Stop()
	{
		if(Input.GetMouseButtonUp(0))
			StopCoroutine("StartPathFinding");
	}
	
	void Move()
	{
		if(m_ai.m_agression == AI.Agression.PASSIVE)
			return;

		m_player = Player.GetClosestPlayer(transform.position, m_attackRange);

		if(m_player == null)
			return;

		if(m_ai.PathBlocked(m_player.transform.position))
		{
			if(!m_pathFinding)
			{Debug.LogWarning("Start Path Finding");
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

		m_ai.FindTarget(m_player.transform.position);
	}
	
	IEnumerator StartPathFinding()
	{
		while(true)
		{
			if(m_player == null)
				StopCoroutine("StartPathFinding");
			else
				m_ai.FindPath(m_player.transform.position);
			
			yield return new WaitForSeconds(m_pathUpdateTime);
		}
	}
}