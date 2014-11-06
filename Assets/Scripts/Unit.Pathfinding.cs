/// <summary>
/// Handles A* Path Finding....
/// </summary>
/// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public partial class Unit : Actor
{
	List<Cell> m_path = new List<Cell>();

	//Stores cells that may fall along path...
	List<Cell> m_openList = new List<Cell>();
	//Stores cells that will be looked at again later...
	List<Cell> m_visitedList = new List<Cell>();

	static int m_maxSearchDepth = 80;

	public void StartFindingPath()
	{
		ClearPath();

		Cell parent = ActivePath.GetCell(transform.position);

		parent.m_parent = null;
		ActivePath.SetCellH(parent, m_targetCell);
		parent.G = 0;

		m_targetCell = FindPath(parent);

		if(m_targetCell == null)
			return;

		while(m_targetCell != null)
		{
			m_path.Add(m_targetCell);
			m_targetCell = m_targetCell.m_parent;
		}

		m_targetCell = m_path[0];

		m_path.Reverse();
		m_path.RemoveAt(0);
	}

	public void ClearPath()
	{
		m_path.Clear();
		m_openList.Clear();
		m_visitedList.Clear();
	}

	Cell FindPath(Cell nextCell)
	{
		if(nextCell == m_targetCell)
			return m_targetCell;

		m_visitedList.Add(nextCell);

		AddChild(Direction.LEFT);
		AddChild(Direction.RIGHT);
		AddChild(Direction.FORWARD);
		AddChild(Direction.BACK);
		AddChild(Direction.LEFT_FORWARD);
		AddChild(Direction.RIGHT_FORWARD);
		AddChild(Direction.LEFT_BACK);
		AddChild(Direction.RIGHT_BACK);

		if(m_openList.Count > 0)
		{
			if(m_openList.Count > m_maxSearchDepth)
				return m_targetCell;

			return FindPath(FindLowestF());
		}

		return null;
	}

	void AddChild(Direction direction)
	{
		Cell parent = m_visitedList[m_visitedList.Count - 1];
		Cell child = ActivePath.GetNeighbor(parent, direction);

		if(child == null || m_visitedList.Contains(child))
			return;
		
		if(!ActivePath.HeightInRange(parent, child, m_walkableHeight))
			return;

		if(!ActivePath.CornerHeightInRange(parent, direction, m_walkableHeight))
			return;
		
		if(m_openList.Contains(child))
		{
			int newGDist = parent.G + ActivePath.SetManhatDist(direction);
			if(newGDist < child.G)
			{
				child.G = newGDist;
				child.m_parent = parent;
			}
		}
		else
		{
			ActivePath.SetCellGH(child, m_targetCell, direction);
			child.m_parent = parent;
			child.G += parent.G;
			m_openList.Add(child);
		}
	}

	Cell FindLowestF()
	{
		int index = 0;

		for(int i = 0; i < m_openList.Count; ++i)
		{
			if(m_openList[i].F < m_openList[index].F)
				index = i;
		}

		Cell cell = m_openList[index];
		m_openList.RemoveAt(index);

		return cell;
	}

	void DebugDraw()
	{
		if(m_debug)
		{
			foreach(Cell child in m_openList)
				Debug.DrawLine(child.Position, child.Position + Vector3.up, Color.red);
			
			foreach(Cell child in m_visitedList)
				Debug.DrawLine(child.Position, child.Position + Vector3.up, Color.yellow);
			
			foreach(Cell child in m_path)
				Debug.DrawLine(child.Position, child.Position + Vector3.up, Color.green);
		}
	}

	Ray m_ray = new Ray();
	RaycastHit m_hit = new RaycastHit();

	public bool PathBlocked(Vector3 targetPoint)
	{
		Vector3 direction = targetPoint - transform.position;
		m_ray = new Ray(transform.position + Vector3.up, direction.normalized);
		float radius = m_moveThreshold/2.0f;
		
		if(m_debug)
			Debug.DrawRay(transform.position + Vector3.up, direction, Color.blue);

		if(Physics.SphereCast(m_ray, radius, out m_hit, direction.magnitude, m_physicsLayer))
			return true;

//		if(Physics.Raycast(m_ray, out m_hit, direction.magnitude, m_physicsLayer))
//			return true;
		
		return false;
	}


	Path ActivePath
	{
		get { return Grid.Instance.m_path; }
	}
}
