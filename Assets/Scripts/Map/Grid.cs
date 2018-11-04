using ch.sycoforge.Decal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public GameController gc;

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius = 0.5f;
	public Node[,] grid;
    public List<GameObject> tiles;
    public List<GameObject> highlightedTiles;
	public List<Node> path;
    public List<Node> range;
    public Terrain terrain;

    // Directions
    public Vector3 rightDirection;
    public Vector3 leftDirection { get { return -rightDirection; } }
    public Vector3 forwardDirection;
    public Vector3 backwardDirection { get { return -forwardDirection; } }

    float nodeDiameter;
	int gridSizeX, gridSizeY;

    void Awake()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Map"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Grid"));
    }

	public int MaxSize
    {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    public void CreateGridDep()
    {
        nodeDiameter = nodeRadius * 2;

        gridWorldSize = new Vector2(10f, 10f);
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        rightDirection = gc.protag.transform.rotation * Vector3.right;
        forwardDirection = gc.protag.transform.rotation * Vector3.forward;

        grid = new Node[gridSizeX, gridSizeY];

        GameObject tileGO = Resources.Load("Prefabs/TileDecal") as GameObject;

        Vector3 bottomRight = gc.protag.transform.position + rightDirection * (Mathf.RoundToInt((gridSizeX - 1) / 2) * nodeDiameter) + forwardDirection * nodeRadius;
        Vector3 cellPoint;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cellPoint = bottomRight + leftDirection * x + forwardDirection * y;

                Node node = new Node(cellPoint, x, y);

                EasyDecal decalInstance = EasyDecal.ProjectAt(tileGO, terrain.gameObject, cellPoint, Quaternion.identity);
                decalInstance.gameObject.name = "(" + x.ToString() + " , " + y.ToString() + ")";
                decalInstance.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.forwardDirection, Vector3.up);
                decalInstance.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                Tile tile = decalInstance.gameObject.GetComponent<Tile>();

                tiles.Add(decalInstance.gameObject);
                tile.grid = gc.grid;
                tile.node = node;
                node.tile = tile;
                grid[x, y] = node;
            }
        }

    }

    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        
        gridWorldSize = new Vector2(10f, 10f);
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        rightDirection = gc.protag.transform.rotation * Vector3.right;
        forwardDirection = gc.protag.transform.rotation * Vector3.forward;

        grid = new Node[gridSizeX, gridSizeY];
        List<Point> cellPath = new List<Point>();
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x <gridSizeX; x++)
            {
                int newX = y % 2 == 0 ? x : 9 - x;
                cellPath.Add(new Point(newX, y));
                
            }
        }
        List<Vector3> pathDirections = GetPathDirections(cellPath);

        GameObject tileGO = Resources.Load("Prefabs/TileDecal") as GameObject;
        Vector3 bottomRight = gc.protag.transform.position + rightDirection * (Mathf.RoundToInt((gridSizeX - 1) / 2) * nodeDiameter) + forwardDirection * nodeRadius + rightDirection * nodeRadius ;
        Vector3 cellPoint = bottomRight;

        for (int i = 0; i < cellPath.Count; i++)
        {
            Point cell = cellPath[i];

            int x = Mathf.RoundToInt(cell.x);
            int y = Mathf.RoundToInt(cell.y);

            Node node = new Node(cellPoint, x, y);

            EasyDecal decalInstance = EasyDecal.ProjectAt(tileGO, terrain.gameObject, cellPoint, Quaternion.identity);
            decalInstance.gameObject.name = "(" + x.ToString() + " , " + y.ToString() + ")";
            decalInstance.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.forwardDirection, Vector3.up);
            decalInstance.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            Tile tile = decalInstance.gameObject.GetComponent<Tile>();

            cellPoint = MoveAlongTerrain(cellPoint, pathDirections[i], nodeDiameter);
        }

    }

    public Vector3 MoveAlongTerrain(Vector3 startPoint, Vector3 direction, float distance = 10f, int steps = 4)
    {
        float step = distance / steps;
        float stepSquared = step * step;
        float distanceTraveled = 0f;
        Vector3 endPoint = startPoint;

        for (int i = 0; i < steps; i++)
        {
            endPoint = endPoint + direction * step;
            float height = terrain.SampleHeight(endPoint) + nodeRadius;
            float verticalDistance = Mathf.Abs(endPoint.y - height);

            endPoint.y = height;
            if(verticalDistance > 0)
            {
                distanceTraveled += Mathf.Sqrt(stepSquared + verticalDistance * verticalDistance);
            } else
            {
                distanceTraveled += step;
            }

            if (distanceTraveled >= distance)
                break;
        }
        return endPoint;
    }

    private List<Vector3> GetPathDirections(List<Point> cellPath)
    {
        List<Vector3> pathDirection = new List<Vector3>();

        for (int i = 0; i < cellPath.Count-1; i++)
        {
            Point cell = cellPath[i];
            Point nextCell = cellPath[i + 1];

            if(nextCell.y - cell.y != 0)
            {
                pathDirection.Add(forwardDirection);
                continue;
            } else
            {
                if(nextCell.x - cell.x > 0)
                {
                    pathDirection.Add(leftDirection);
                } else
                {
                    pathDirection.Add(rightDirection);
                }
            }

        }

        return pathDirection;
    }

    public Node FindNearestNode(Vector3 worldPosition, float lowestDistance = 1f)
    {
        Node closestNode = null;

        foreach (Node node in gc.grid.grid)
        {
            float nodeDistance = Vector3.Distance(worldPosition, node.worldPosition);
            if (nodeDistance < lowestDistance)
            {
                lowestDistance = nodeDistance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public void ClearGrid()
    {

        grid = null;

        foreach(GameObject tileGO in tiles)
        {
            DestroyImmediate(tileGO, true);
        }
    }

    public void SelectNodes(List<Node> nodes, Color color)
    {
        foreach (Node node in nodes)
        {
            GameObject highlightedGO = Resources.Load("Prefabs/HighlightDecal") as GameObject;
            EasyDecal highlightedDecal = highlightedGO.GetComponent<EasyDecal>();

            EasyDecal decalInstance = EasyDecal.ProjectAt(highlightedDecal.gameObject, terrain.gameObject, node.worldPosition, Quaternion.identity);
            //decalInstance.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            decalInstance.gameObject.layer = LayerMask.NameToLayer("Grid");
            decalInstance.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.forwardDirection, Vector3.up);
            highlightedTiles.Add(decalInstance.gameObject);
            
        }
    }

    public void SelectNodes(Node node, Color color)
    {
        GameObject highlightedGO = Resources.Load("Prefabs/HighlightDecal") as GameObject;
        EasyDecal highlightedDecal = highlightedGO.GetComponent<EasyDecal>();

        EasyDecal decalInstance = EasyDecal.ProjectAt(highlightedDecal.gameObject, terrain.gameObject, node.worldPosition, Quaternion.identity);
        //decalInstance.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        decalInstance.gameObject.layer = LayerMask.NameToLayer("Grid");
        decalInstance.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.forwardDirection, Vector3.up);
        highlightedTiles.Add(decalInstance.gameObject);
    }

    public void DeSelectNodes()
    {
        foreach(GameObject tile in highlightedTiles)
        {
            DestroyImmediate(tile, true);
        }
    }

    public void HighlightNodes(List<Node> nodes, Color color)
    {
        foreach (Node node in nodes)
        {
            GameObject go = TileFromNode(node).gameObject;
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = color;
            outline.OutlineWidth = 5f;
        }
    }

    public void UnHighlightNodes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            GameObject go = TileFromNode(node).gameObject;
            Destroy(go.GetComponent<Outline>());
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

    public List<Node> GetNeighbors(Node node, bool diag, bool ignoreOccupant) {
		List<Node> neighbors = new List<Node> ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0 || (Mathf.Abs(x) == Mathf.Abs(y) && !diag))
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    Node neighbor = grid[checkX, checkY];
                    if (neighbor.occupant != null && !ignoreOccupant)
                        continue;
					neighbors.Add (neighbor);
				}
			}
		}

		return neighbors;
	}

    /*
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x, y];
    }
    */

	public Tile TileFromNode(Node node) {

        if(node.tile != null)
        {
            return node.tile;
        }
        int x = node.gridX;
		int y = node.gridY;
		string goName = "(" + x.ToString () + " , " + y.ToString () + ")";
		Tile tile = GameObject.Find (goName).GetComponent<Tile>();

        return tile;
    }
	
    public void ColorTiles()
    { 
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
            else
            {
                go.GetComponent<Renderer>().material.color = Color.grey;
            }
        }
    }

    /*
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (grid != null) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
			}
		}
	}
    */
}