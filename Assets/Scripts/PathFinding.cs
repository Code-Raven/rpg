/// <summary>
/// A* Algorithmic System...
/// </summary>

using UnityEngine;
using System.Collections;
using Utility;

namespace PathFinding
{
	public enum Direction {LEFT, RIGHT, FORWARD, BACK, LEFT_FORWARD,
		RIGHT_FORWARD, LEFT_BACK, RIGHT_BACK};

	public class Cell
	{
		public Cell m_parent = null;
		Vector3 m_position = Vector3.zero;
		Vector3 m_normal = Vector3.zero;

		int m_index = 0, m_x = 0, m_z = 0;
		int m_g = 0;	//Distance from start to n cell, given path...
		int m_h = 0;	//Heuristic distance from n cell to goal...

		public Cell(Vector3 position, Vector3 normal, int index, int x, int z)
		{
			m_position = position;
			m_normal = normal;

			m_index = index;
			m_x = x;
			m_z = z;
		}

		public int Index
		{
			get { return m_index; }
		}
		
		public int X
		{
			get { return m_x; }
		}
		
		public int Z
		{
			get { return m_z; }
		}

		public int G
		{
			get { return m_g; }
			set { m_g = value; }
		}
		
		public int H
		{
			get { return m_h; }
			set { m_h = value; }
		}
		
		public int F
		{
			get { return m_g + m_h; }
		}
		
		public Vector3 Position
		{
			get { return m_position; }
		}

		public Vector3 Normal
		{
			get { return m_position; }
		}
	}

	public class Path
	{
		int m_width = 6, m_depth = 6, m_size = 36;
		Vector3 m_gridPos = Vector3.zero;
		float m_cellSize = 1;

		Cell[] m_cells = null;

		public Path(int width, int depth, Vector3 gridPos, float cellSize, int layerMask)
		{
			m_width = width;
			m_depth = depth;
			m_gridPos = gridPos;
			m_cellSize = cellSize;

			CreateCells(layerMask);
		}

		void CreateCells(LayerMask layerMask)
		{
			m_size = m_depth * m_width;
			m_cells = new Cell[m_size];

			Vector3 cellPos = Vector3.zero;
			Vector3 cellNorm = Vector3.up;

			for(int i = 0; i < m_cells.Length; ++i)
			{
				int x = 0, z = 0;
				SetCellXY(i, out x, out z);

				cellPos = GetCellPosition(x, z);

				Vector3 projDir = Vector3.down * 1000.0f;
				
				RaycastHit hit = new RaycastHit();

				if(Physics.Raycast(cellPos - projDir, projDir.normalized,
					out hit, projDir.magnitude, layerMask))
				{
					cellPos = hit.point;
					cellNorm = hit.normal;
				}

				m_cells[i] = new Cell(cellPos, cellNorm, i, x, z);
			}
		}

		Cell GetCell(int x, int z)
		{
			int cellIndex = GetCellIndex(x, z);

			if(cellIndex < 0)
				return null;

			return GetCell(cellIndex);
		}

		public Cell GetCell(int cellIndex)
		{
			return m_cells[cellIndex];
		}

		public Cell GetCell(Vector3 position)
		{
			int cellIndex = GetCellIndex(position);

			if(cellIndex < 0)
				return null;

			return m_cells[cellIndex];
		}

		public int GetCellIndex(Vector3 position)
		{
			float halfScale = m_cellSize/2.0f;
			float halfWidth = m_width/2.0f;
			float halfDepth = m_depth/2.0f;
		
			int x = (int)(position.x + halfWidth);
			int z = (int)(position.z + halfDepth);
			
			return GetCellIndex(x, z);
		}

		public int GetCellIndex(int x, int z)
		{
			if(x < 0 || x >= m_width)
				return -1;
			if(z < 0 || z >= m_depth)
				return - 1;
			
			return ((z * m_width) + x);
		}

		Vector3 GetCellPosition(int x, int z)
		{
			float halfScale = m_cellSize/2.0f;
			float halfWidth = m_width/2.0f;
			float halfDepth = m_depth/2.0f;
			
			Vector3 pos = m_gridPos;
			pos.x = ((x - halfWidth) * m_cellSize) + halfScale;
			pos.z = ((z - halfDepth) * m_cellSize) + halfScale;
			
			return pos;
		}

		void SetCellXY(int cellIndex, out int x, out int z)
		{
			x = cellIndex % m_width;
			z = cellIndex / m_width;
		}

		public void SetCellXY(Vector3 cellPos, out int x, out int z)
		{
			SetCellXY(GetCell(cellPos).Index, out x, out z);
		}

		//LEFT, RIGHT, FORWARD, BACK, LEFT_FORWARD, RIGHT_FORWARD, LEFT_BACK, RIGHT_BACK...

		static Vector2[] m_nextDir = new Vector2[8]
		{
			new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1),
			new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1)
		};

		//Index into, direction greater than BACK. Subtract 4 from direction to get second index...
		static int[] m_dirIndex = new int[8]
		{
			0, 1, 0, 1, 2, 2, 3, 3
		};

		public Cell GetNeighbor(Cell cell, Direction direction)
		{
			Vector2 dir = m_nextDir[(int)direction];
			Cell neighbor = GetCell(cell.X + (int)dir.x, cell.Z + (int)dir.y);
			
			return neighbor;
		}

		public void SetCellGH(Cell cell, Cell lastCell, Direction direction)
		{
			SetCellG(cell, direction);
			SetCellH(cell, lastCell);
		}

		public void SetCellG(Cell cell, Direction direction)
		{
			cell.G = SetManhatDist(direction);
		}

		public int SetManhatDist(Direction direction)
		{
			return ((int)direction > (int)Direction.BACK)
				? DiagManhatDist : HorizManhatDist;
		}

		public void SetCellH(Cell cell, Cell lastCell)
		{
			cell.H = (Mathf.Abs(cell.X - lastCell.X) +
				Mathf.Abs(cell.Z - lastCell.Z)) * HorizManhatDist;
		}

		public CellEventHandler CellTarget = null;

		public void UpdateCellTarget()
		{
			if(CellTarget == null)
				return;

			foreach(Cell cell in m_cells)
				CellTarget(cell);
		}

		public bool HeightInRange(Cell cell, Cell neighbor, float maxHeight)
		{
			float height = Mathf.Abs(cell.Position.y - neighbor.Position.y);
			return !(height > maxHeight);
		}

		public bool CornerHeightInRange(Cell parent, Direction direction, float maxHeight)
		{
			int stride = 4;
			int firstIndex = (int)direction;
			int secIndex = firstIndex - stride;

			if(secIndex < 0)
				return true;

			Direction firstDir = (Direction)m_dirIndex[firstIndex];
			Direction secDir = (Direction)m_dirIndex[secIndex];

			Cell firstNeighbor = GetNeighbor(parent, firstDir);
			Cell secNeighbor = GetNeighbor(parent, secDir);

			return (HeightInRange(parent, firstNeighbor, maxHeight) &&
				HeightInRange(parent, secNeighbor, maxHeight));
		}

		public int Size
		{
			get { return m_size; }
		}

		public int HorizManhatDist
		{
			get { return 10; }
		}

		public int DiagManhatDist
		{
			get { return 14; }
		}
	}
}
