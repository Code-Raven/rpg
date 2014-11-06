using UnityEngine;
using System.Collections;
using PathFinding;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
	public bool m_debug = false;
	public int m_width = 6;
	public int m_depth = 6;

	public Path m_path = null;
	
	Mesh m_mesh = null;

	static Grid m_instance = null;

	void Awake()
	{
		if(!Application.isPlaying)
			return;

		m_instance = this;
		CreateMesh();
	}

	void Update()
	{
		if(Application.isPlaying)
		{
			m_path.UpdateCellTarget();
			return;
		}

		m_width = m_width % 2 == 0 ? (m_width > 0 ? m_width : 2) : m_width + 1;
		m_depth = m_depth % 2 == 0 ? (m_depth > 0 ? m_depth : 2) : m_depth + 1;
	}
		
	public void CreateMesh()
	{
		m_mesh = new Mesh();

		m_mesh.name = "Grid";

		float halfWidth = m_width/2.0f;
		float halfDepth = m_depth/2.0f;

		m_mesh.vertices = new Vector3[4]
		{
			new Vector3(-halfWidth, 0.0f, -halfDepth),
			new Vector3(halfWidth, 0.0f, -halfDepth),
			new Vector3(halfWidth, 0.0f, halfDepth),
			new Vector3(-halfWidth, 0.0f, halfDepth)
		};

		m_mesh.uv = new Vector2[]
		{
			new Vector2 (0.0f, 0.0f),
			new Vector2 (0.0f, 1.0f),
			new Vector2(1.0f, 1.0f),
			new Vector2 (1.0f, 0.0f)
		};

		//m_mesh.triangles = new int[6]{0, 1, 2, 0, 2, 3};	//Flipped render direction...
		m_mesh.triangles = new int[6]{0, 3, 2, 2, 1, 0};
		m_mesh.RecalculateNormals();
		m_mesh.RecalculateBounds();

		GetComponent<MeshFilter>().mesh = m_mesh;
		GetComponent<MeshCollider>().sharedMesh = m_mesh;

		renderer.material.mainTextureScale = new Vector2(m_depth, m_width);

		m_path = new Path(m_width, m_depth, transform.position, 1.0f, 1 << gameObject.layer);

		if(m_debug)
			m_path.CellTarget = DebugCell;
	}

	public void DebugCell(Cell cell)
	{
		Debug.DrawLine(cell.Position, cell.Position + Vector3.up, Color.green);
	}

	public static Grid Instance
	{
		get { return m_instance; }
	}
}
