using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

[RequireComponent(typeof(CharacterController))]
public partial class Unit : Actor
{
	public bool m_debug = false;
	public LayerMask m_physicsLayer = new LayerMask();
	public float m_travelSpeed = 5.0f;	//Meters per second...

	public Cell m_targetCell = null;

	public static float m_walkableHeight = 0.25f;

	float m_moveThreshold = 0.5f;
	float m_pathTurnSpeed = 10.0f;
	float m_turnSpeed = 20.0f;

	CharacterController m_charControl = null;

	// Use this for initialization
	public virtual void Awake()
	{
		m_charControl = GetComponent<CharacterController>();
		m_moveThreshold = transform.localScale.x/2.0f;
		FindTarget(transform.position);
	}

	void Update()
	{
		DebugDraw();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if(m_targetCell == null)
			return;

		if(!FindPath())
			FindTarget();
	}

	void Move(Vector3 forward)
	{
		m_charControl.Move(forward * m_travelSpeed * Time.fixedDeltaTime);
		//transform.position += transform.forward * m_travelSpeed * Time.fixedDeltaTime;
	}

	void LookAtTargetPos(Vector3 targetPos, float turnSpeed)
	{
		Vector3 dir = targetPos - transform.position;
		//dir.y = 0;
		Quaternion rot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.fixedDeltaTime);
	}

	bool FindPath()
	{
		if(m_path.Count == 0)
			return false;

		Vector3 targetPos = m_path[0].Position;

		if(m_path[0] == m_targetCell)
			targetPos += m_posOffset;

		Vector3 lookDirection = (targetPos - transform.position).normalized;
		targetPos.y = transform.position.y;

		if(Vector3.Distance(transform.position, targetPos) < m_moveThreshold)
		{
			m_path.RemoveAt(0);
			return true;
		}

		LookAtTargetPos(targetPos, m_pathTurnSpeed);
		Move(lookDirection);

		return true;
	}

	void FindTarget()
	{
		Vector3 targetPos = m_targetCell.Position + m_posOffset;
		Vector3 lookDirection = (targetPos - transform.position).normalized;
		targetPos.y = transform.position.y;

		if(Vector3.Distance(transform.position, targetPos) < m_moveThreshold)
			return;

		LookAtTargetPos(targetPos, m_turnSpeed);

		Move(lookDirection);
	}

	Vector3 m_posOffset = Vector3.zero;

	public void FindTarget(Vector3 position)
	{
		ClearPath();

		m_targetCell = Grid.Instance.m_path.GetCell(position);

		if(m_targetCell == null)
			return;

		m_posOffset = position - m_targetCell.Position;
	}

	public void FindPath(Vector3 position)
	{
		m_targetCell = Grid.Instance.m_path.GetCell(position);

		if(m_targetCell == null)
			return;

		m_posOffset = position - m_targetCell.Position;

		StartFindingPath();
	}
}
