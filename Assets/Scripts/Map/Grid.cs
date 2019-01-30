using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public BattleController bc;
    [SerializeField]
    private GameObject battleGrid;
    [SerializeField]
    private Projector battleGridProjector;
    public Tile bottomRightTile;

    private int UnWalkableLayerMask;
    public int gridCells;
    private float gridWorldSize;
    public float nodeRadius;
	public Node[,] grid;
    public List<GameObject> tiles;
    public List<GameObject> highlightedTiles;
    public Dictionary<string, List<GameObject>> selectedTiles;
	public List<Node> path;
    public List<Node> range;
    public Vector3 centerPoint;

    // Directions
    public Vector3 rightDirection;
    public Vector3 leftDirection { get { return -rightDirection; } }
    public Vector3 forwardDirection;
    public Vector3 backwardDirection { get { return -forwardDirection; } }

    public enum Position { Front, Back, Left, Right, Diagonal};

    public float nodeDiameter;
	int gridSizeX, gridSizeY;

    void Awake()
    {
        selectedTiles = new Dictionary<string, List<GameObject>>();
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Grid"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("GridClick"));
    }

	public int MaxSize
    {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    public IEnumerator GenerateGrid(Action callback)
    {
        nodeDiameter = bottomRightTile.gameObject.transform.localScale.x;

        gridCells = 10;

        gridWorldSize = gridCells * nodeDiameter;
        gridSizeX = Mathf.RoundToInt(gridWorldSize / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize / nodeDiameter);

        rightDirection = bottomRightTile.transform.rotation * Vector3.right;
        forwardDirection = bottomRightTile.transform.rotation * Vector3.forward;

        grid = new Node[gridSizeX, gridSizeY];
        List<Point> cellPath = new List<Point>();
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                int newX = y % 2 == 0 ? x : gridSizeX - 1 - x;
                cellPath.Add(new Point(newX, y));

            }
        }
        List<Vector3> pathDirections = GetPathDirections(cellPath);
        Vector3 bottomRight = bottomRightTile.transform.position;
        bottomRight.y = FindHeightPoint(bottomRight);
        Vector3 cellPoint = bottomRight;
        for (int i = 0; i < cellPath.Count; i++)
        {
            // Grab next cell in path
            Point cell = cellPath[i];

            // Get coordinates
            int x = Mathf.RoundToInt(cell.x);
            int y = Mathf.RoundToInt(cell.y);

            // Create game object
            GameObject tileGO = FindTileGo(cellPoint);
            tileGO.name = "(" + x.ToString() + " , " + y.ToString() + ")";
            tiles.Add(tileGO.gameObject);
            Tile tile = tileGO.gameObject.GetComponent<Tile>();

            // Find Anchor point
            Vector3 anchorPoint = tileGO.transform.Find("AnchorPoint").transform.position;

            // Create new node
            Node node = new Node(anchorPoint, x, y);

            // Assign appropriate values for tile and node
            tile.grid = bc.grid;
            tile.node = node;
            node.tile = tile;

            // Is tile walkable?
            Collider[] alertColliders = Physics.OverlapSphere(anchorPoint + Vector3.up * nodeRadius, nodeRadius, UnWalkableLayerMask, QueryTriggerInteraction.UseGlobal);
            if (alertColliders != null && alertColliders.Length != 0)
            {
                tile.isWalkable = false;
            }

            // Assign to grid
            grid[x, y] = node;

            // Set up next cellPoint, if not at the end
            if (i >= pathDirections.Count)
                break;

            cellPoint = cellPoint + pathDirections[i] * nodeDiameter;
            cellPoint.y = FindHeightClear(cellPoint, nodeRadius);
            if (i % 3 == 0)
                yield return null;
        }

        callback();
        yield break;
    }

    private void ProjectorSetup()
    {
        battleGridProjector.orthographicSize = gridCells + 1.5f;
        battleGridProjector.transform.position = centerPoint + Vector3.up * 5f;
        battleGridProjector.transform.rotation = Quaternion.LookRotation(Vector3.down, forwardDirection);
        battleGridProjector.gameObject.SetActive(true);
    }

    public Vector3 MoveAlongTerrain(Vector3 startPoint, Vector3 direction, float distance = 1f, int steps = 4)
    {
        float step = distance / steps;
        float stepSquared = step * step;
        float distanceTraveled = 0f;
        Vector3 endPoint = startPoint;

        for (int i = 0; i < steps; i++)
        {
            endPoint = endPoint + direction * step;
            //float height = terrain.SampleHeight(endPoint) + nodeRadius;
            float height = FindHeightPoint(endPoint) + nodeRadius;
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

    public GameObject FindTileGo(Vector3 point)
    {
        int layerMask = (1 << LayerMask.NameToLayer("Grid"));
        layerMask |= (1 << LayerMask.NameToLayer("GridClick"));
        
        RaycastHit hit;
        if (Physics.Raycast(point + Vector3.up * 5f, -Vector3.up, out hit, 100f, layerMask))
        {
            if (hit.collider.tag == "Map")
            {
                return hit.collider.gameObject;
            }
        }
        return new GameObject();
    }

    public float FindHeightPoint(Vector3 point)
    {
        int layerMask = (1 << LayerMask.NameToLayer("Grid"));
        layerMask |= (1 << LayerMask.NameToLayer("GridClick"));
        layerMask |= (1 << LayerMask.NameToLayer("Character"));
        layerMask = ~layerMask;

        RaycastHit hit;
        if(Physics.Raycast(point+Vector3.up*20f, -Vector3.up, out hit, 100f, layerMask))
        {
            if(hit.collider.tag == "Map")
            {
                return hit.point.y;
            }
        }
        return point.y;
    }

    public float FindHeightClear(Vector3 point, float radius)
    {
        Vector3 br = point + rightDirection * radius + backwardDirection * radius;
        Vector3 bl = point + leftDirection * radius + backwardDirection * radius;
        Vector3 fr = point + rightDirection * radius + forwardDirection * radius;
        Vector3 fl = point + leftDirection * radius + forwardDirection * radius;

        float maxHeight = FindHeightPoint(point);
        foreach(Vector3 corner in new Vector3[] { br, bl, fr, fl })
        {
            float _height = FindHeightPoint(corner);
            if (_height > maxHeight)
                maxHeight = _height;
        }

        return maxHeight;
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

    public Vector3 GetDirection(Node startingNode, Node endingNode)
    {
        float xDist = endingNode.gridX - startingNode.gridX;
        float yDist = endingNode.gridY - startingNode.gridY;

        if(Mathf.Abs(xDist) > Mathf.Abs(yDist))
        {
            if (xDist > 0)
                return leftDirection;
            else
                return rightDirection;
        }
        else
        {
            if (yDist > 0)
                return forwardDirection;
            else
                return backwardDirection;
        }

    }

    public Position CompareDirection(Node nodeA, Node nodeB, Vector3 targetDirection)
    {
        // Check for back position
        if ((targetDirection == forwardDirection && nodeA.gridY < nodeB.gridY) ||
            (targetDirection == backwardDirection && nodeA.gridY > nodeB.gridY) ||
            (targetDirection == leftDirection && nodeA.gridX < nodeB.gridX) ||
            (targetDirection == rightDirection && nodeA.gridX > nodeB.gridX))
        {
            return Position.Back;
        }

        // Check for front position
        if ((targetDirection == backwardDirection && nodeA.gridY < nodeB.gridY) ||
            (targetDirection == forwardDirection && nodeA.gridY > nodeB.gridY) ||
            (targetDirection == leftDirection && nodeA.gridX > nodeB.gridX) ||
            (targetDirection == rightDirection && nodeA.gridX < nodeB.gridX))
        {
            return Position.Front;
        }

        // Check for left position
        if ((targetDirection == backwardDirection && nodeA.gridX < nodeB.gridX) ||
            (targetDirection == forwardDirection && nodeA.gridX > nodeB.gridX) ||
            (targetDirection == leftDirection && nodeA.gridY < nodeB.gridY) ||
            (targetDirection == rightDirection && nodeA.gridY > nodeB.gridY))
        {
            return Position.Left;
        }

        // Check for right position
        if ((targetDirection == backwardDirection && nodeA.gridX > nodeB.gridX) ||
            (targetDirection == forwardDirection && nodeA.gridX < nodeB.gridX) ||
            (targetDirection == leftDirection && nodeA.gridY > nodeB.gridY) ||
            (targetDirection == rightDirection && nodeA.gridY < nodeB.gridY))
        {
            return Position.Right;
        }

        return Position.Diagonal;

    }

    public Node FindNearestNode(Vector3 worldPosition, float lowestDistance = 1f)
    {
        float normalizedLowestDistance = lowestDistance * nodeDiameter;
        Node closestNode = null;
        worldPosition.y = FindHeightPoint(worldPosition);
        foreach (Node node in bc.grid.grid)
        {
            float nodeDistance = Vector3.Distance(worldPosition, node.worldPosition);
            if (nodeDistance < normalizedLowestDistance)
            {
                normalizedLowestDistance = nodeDistance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public void ClearGrid()
    {
        battleGrid.gameObject.SetActive(false);
        battleGridProjector.gameObject.SetActive(false);
        grid = null;

        foreach(GameObject tileGO in tiles)
        {
            DestroyImmediate(tileGO, true);
        }
    }

    public void SelectNodes(List<Node> nodes, Color color, string set)
    {
        foreach (Node node in nodes)
        {
            SelectNodes(node, color, set);
        }
    }

    public void SelectNodes(Node node, Color color, string set)
    {
        GameObject nodeGO = Resources.Load("Prefabs/Grid/SelectedTile") as GameObject;
        GameObject nodeInstance = Instantiate(nodeGO, node.worldPosition + Vector3.up * 0.1f, Quaternion.identity, GameObject.Find("BattleGrid").transform);
        nodeInstance.transform.localScale = nodeInstance.transform.localScale * nodeDiameter;
        nodeInstance.layer = LayerMask.NameToLayer("Grid");
        nodeInstance.transform.rotation = Quaternion.LookRotation(bc.grid.forwardDirection, Vector3.up);
        nodeInstance.name = set;

        MeshRenderer mesh = nodeInstance.GetComponent<MeshRenderer>();
        mesh.material = new Material(mesh.material);
        mesh.material.SetColor("_Color", color);

        // Add to dictionary of selected tile lists
        if (!selectedTiles.ContainsKey(set))
        {
            selectedTiles[set] = new List<GameObject>();
        }
        selectedTiles[set].Add(nodeInstance);
    }

    public void DeSelectNodes(string set)
    {
        if (!selectedTiles.ContainsKey(set))
            return;

        foreach (GameObject tile in selectedTiles[set])
        {
            DestroyImmediate(tile, true);
        }
        selectedTiles[set].Clear();
    }

    public void HighlightPath(Node startingNode, List<Node> nodes, Color color)
    {
        bc.lineRenderer.positionCount = nodes.Count + 1;
        Vector3[] points = new Vector3[nodes.Count + 1];
        points[0] = startingNode.tile.WorldPosition + Vector3.up * bc.CurrentCharacter.height;
        for (int i = 0; i < nodes.Count; i++)
        {
            points[i + 1] = nodes[i].worldPosition + Vector3.up * bc.CurrentCharacter.height;
        }
        bc.lineRenderer.SetPositions(points);
    }

    public void HighlightPath(Node startingNode, Node node, Color color)
    {
        bc.lineRenderer.positionCount = 2;
        Vector3[] points = new Vector3[2];
        points[0] = startingNode.tile.WorldPosition + Vector3.up * bc.CurrentCharacter.height;
        points[1] = node.worldPosition + Vector3.up * bc.CurrentCharacter.height;
        bc.lineRenderer.SetPositions(points);
    }

    public void HighlightNodes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            //GameObject highlightedGO = Resources.Load("Prefabs/Grid/HighlightedTile") as GameObject;
            GameObject highlightedGO = Resources.Load("Prefabs/Grid/GridTileVisible") as GameObject;

            GameObject hlInstance = Instantiate(highlightedGO, node.worldPosition + Vector3.up * 0.1f, Quaternion.identity, GameObject.Find("BattleGrid").transform);
            hlInstance.transform.rotation = Quaternion.LookRotation(Vector3.up, bc.grid.forwardDirection);
            hlInstance.transform.localScale = hlInstance.transform.localScale * nodeDiameter;
            //decalInstance.transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
            hlInstance.layer = LayerMask.NameToLayer("Grid");
            highlightedTiles.Add(hlInstance);

        }
    }

    public void UnHighlightNodes(List<Node> nodes)
    {
        foreach (GameObject tile in highlightedTiles)
        {
            DestroyImmediate(tile, true);
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