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
    public float nodeRadius
    {
        get { return nodeDiameter / 2; }
    }
    public float nodeDiameter;
    public Node[,] grid;
    public List<GameObject> tiles;
    public List<GameObject> highlightedTiles;
    public Dictionary<string, List<Tile>> selectedTiles;
	public List<Node> path;
    public Vector3 centerPoint;

    // Directions
    public Vector3 rightDirection;
    public Vector3 leftDirection { get { return -rightDirection; } }
    public Vector3 forwardDirection;
    public Vector3 backwardDirection { get { return -forwardDirection; } }

    public enum Position { Front, Back, Left, Right, Diagonal};

	int gridSizeX, gridSizeY;
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        selectedTiles = new Dictionary<string, List<Tile>>();
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Grid"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("GridClick"));
        UnWalkableLayerMask = 1 << LayerMask.NameToLayer("Unwalkable");
    }

    public IEnumerator GenerateGrid(Action callback)
    {
        nodeDiameter = Mathf.Round(bottomRightTile.GetComponent<BoxCollider>().bounds.extents.x * 2);

        gridCells = 10;

        gridWorldSize = gridCells * nodeDiameter;
        gridSizeX = Mathf.RoundToInt(gridWorldSize / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize / nodeDiameter);

        rightDirection = bottomRightTile.transform.rotation * Vector3.left;
        forwardDirection = bottomRightTile.transform.rotation * Vector3.back;

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
        Vector3 bottomRight = bottomRightTile.transform.Find("AnchorPoint").transform.position;
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

            if(tile == null)
            {
                grid[x, y] = null;
            }
            else
            {
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
            }

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
        if (Physics.Raycast(point + Vector3.up * 50f, -Vector3.up, out hit, 100f, layerMask))
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
        //layerMask |= (1 << LayerMask.NameToLayer("Character"));
        //layerMask = ~layerMask;

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

    public Position CompareDirection(Node fromNode, Node toNode, Vector3 targetFacingDirection)
    {
        // Check for back position
        if ((targetFacingDirection == forwardDirection && fromNode.gridY < toNode.gridY) ||
            (targetFacingDirection == backwardDirection && fromNode.gridY > toNode.gridY) ||
            (targetFacingDirection == leftDirection && fromNode.gridX < toNode.gridX) ||
            (targetFacingDirection == rightDirection && fromNode.gridX > toNode.gridX))
        {
            return Position.Back;
        }

        // Check for front position
        if ((targetFacingDirection == backwardDirection && fromNode.gridY < toNode.gridY) ||
            (targetFacingDirection == forwardDirection && fromNode.gridY > toNode.gridY) ||
            (targetFacingDirection == leftDirection && fromNode.gridX > toNode.gridX) ||
            (targetFacingDirection == rightDirection && fromNode.gridX < toNode.gridX))
        {
            return Position.Front;
        }

        // Check for left position
        if ((targetFacingDirection == backwardDirection && fromNode.gridX < toNode.gridX) ||
            (targetFacingDirection == forwardDirection && fromNode.gridX > toNode.gridX) ||
            (targetFacingDirection == leftDirection && fromNode.gridY < toNode.gridY) ||
            (targetFacingDirection == rightDirection && fromNode.gridY > toNode.gridY))
        {
            return Position.Left;
        }

        // Check for right position
        if ((targetFacingDirection == backwardDirection && fromNode.gridX > toNode.gridX) ||
            (targetFacingDirection == forwardDirection && fromNode.gridX < toNode.gridX) ||
            (targetFacingDirection == leftDirection && fromNode.gridY > toNode.gridY) ||
            (targetFacingDirection == rightDirection && fromNode.gridY < toNode.gridY))
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
            if (node == null)
                continue;
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
        node.tile.AddColor(set, color);
        // Add to dictionary of selected tile lists
        if (!selectedTiles.ContainsKey(set))
        {
            selectedTiles[set] = new List<Tile>();
        }
        selectedTiles[set].Add(node.tile);

    }

    public void DeSelectNodes(string set)
    {
        if (!selectedTiles.ContainsKey(set))
            return;

        foreach (Tile tile in selectedTiles[set])
        {
            tile.RemoveColor(set);
            //DestroyImmediate(tile, true);
        }
        selectedTiles[set].Clear();
    }

    public void ResetCosts()
    {
        foreach (Node node in grid)
        {
            if (node == null)
                continue;
            node.gCost = 0;
            node.hCost = 0;
        }
    }

    public List<Node> GetNeighbors(Node node, bool diag, bool ignoreOccupant, bool ignoreMoveBlock) {
		List<Node> neighbors = new List<Node> ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0 || (Mathf.Abs(x) == Mathf.Abs(y) && !diag))
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    Node neighbor = grid[checkX, checkY];

                    // Invalid neighbor conditions

                    // Not a tile
                    if (neighbor == null)
                        continue;

                    // Occupied tile and can't ignore occupants
                    if (neighbor.occupant != null && !ignoreOccupant)
                        continue;

                    // Neighbor is blocked from node to neighbor
                    if (!ignoreMoveBlock && node.tile.moveBlocks.Contains(CompareDirection(neighbor, node, forwardDirection)))
                        continue;

                    // Neighbor is blocked from neighbor to node
                    if (!ignoreMoveBlock && neighbor.tile.moveBlocks.Contains(CompareDirection(node, neighbor, forwardDirection)))
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

}