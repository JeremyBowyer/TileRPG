using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public Node[,] grid;
	public List<Node> path;
    public List<Node> range;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	public MapGenerator mapGenerator;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        mapGenerator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        gridWorldSize = mapGenerator.mapSize;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();

        // Color tiles for prototype
        foreach (Node n in grid)
        {
            GameObject go = TileFromNode(n).gameObject;
            if (LayerMask.LayerToName(go.layer) == "Unwalkable")
            {
                go.GetComponent<Renderer>().material.color = Color.red;
            }
            else if (go.tag == "StartingTilePlayer")
            {
                go.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid() {
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask));
				Node node = new Node (walkable, worldPoint, x, y);
                TileFromNode(node).node = node;
                grid[x, y] = node;

            }
		}
	}

    public void SelectTiles(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            GameObject go = TileFromNode(node).gameObject;
            go.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }

    public void DeSelectTiles(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            GameObject go = TileFromNode(node).gameObject;
            go.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void ResetRange()
    {
        range = null;
    }

    public void ResetCosts()
    {
        foreach (Node node in grid)
        {
            node.gCost = 0;
            node.hCost = 0;
        }
    }

    public List<Node> GetNeighbors(Node node) {
		List<Node> neighbors = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0 || Mathf.Abs(x) == Mathf.Abs(y))
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbors.Add (grid [checkX, checkY]);
				}
			}
		}

		return neighbors;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x, y];
	}

	public Tile TileFromNode(Node node) {
		int x = node.gridX;
		int y = node.gridY;
		string goName = "(" + x.ToString () + " , " + y.ToString () + ")";
		Tile tile = GameObject.Find (goName).GetComponent<Tile>();
		return tile;
	}
		
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (grid != null) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
			}
		}
	}
}
