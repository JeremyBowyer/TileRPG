using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// explanation: https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
// Note: this script is a modified example of the above tutorial. Not everything in this script will
// be explained in the above video/tutorial.

public class Pathfinding : MonoBehaviour {

	Grid grid;

	void Awake(){
		grid = GetComponent<Grid> ();
	}

	void Update() {

	}

    public bool CheckRange(Node startNode, Node targetNode, int _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable)
    {
        List<Node> _path = FindPath(startNode, targetNode, _limit, diag, ignoreOccupant, ignoreUnwalkable, false);

        return _path.Count > 0;
    }

    public List<Node> FindRange(Node startNode, int _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable, bool includeOrigin)
    {
        List<Node> nodesInRange = new List<Node>();
        //Node startNode = grid.NodeFromWorldPoint(startPos);

        // First pass. Are nodes even close enough to consider?
        foreach (Node node in grid.grid)
        {
            int distance = GetDistance(startNode, node);
            if (distance <= _limit)
            {
                nodesInRange.Add(node);
            }
        }

        // Second pass. 
        List<Node> validNodesInRange = new List<Node>();
        for (int i = 0; i < nodesInRange.Count; i++)
        {
            Node node = nodesInRange[i];
            List<Node> _path = FindPath(startNode, node, _limit, diag, ignoreOccupant, ignoreUnwalkable, includeOrigin);
            if (_path.Contains(node))
            {
                validNodesInRange.Add(node);
            }
        }

        return validNodesInRange;

    }

	public List<Node> FindPath(Node startNode, Node targetNode, int _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable, bool includeOrigin) {

        ResetCosts();

		//Node startNode = grid.NodeFromWorldPoint (startPos);
		//Node targetNode = grid.NodeFromWorldPoint (targetPos);

        Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);
		while (openSet.Count > 0) {
		
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add (currentNode);

            if (currentNode == targetNode) {
                return RetracePath(startNode, currentNode, _limit, includeOrigin);
            }

			foreach (Node neighbor in grid.GetNeighbors(currentNode, diag, ignoreOccupant)) {
				if ((!neighbor.walkable && !ignoreUnwalkable) || closedSet.Contains(neighbor)) {
					continue;
                }

				int newMovementCostToNeighbor = currentNode.gCost + GetDistance (currentNode, neighbor);
				if( newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
					neighbor.gCost = newMovementCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode);
					neighbor.parent = currentNode;

					if (!openSet.Contains (neighbor)) {
						openSet.Add (neighbor);
					} else {
						openSet.UpdateItem (neighbor);
					}
				}

			}
		}
        return new List<Node>();
	}

	List<Node> RetracePath(Node startNode, Node endNode, int _limit, bool includeOrigin) {

		List<Node> _path = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			_path.Add (currentNode);
			currentNode = currentNode.parent;
		}

        if (includeOrigin)
            _path.Add(startNode);

		_path.Reverse ();

        List<Node> validPath = new List<Node>();
        for(int i = 0; i < _path.Count; i++)
        {
            Node node = _path[i];
            if (node.gCost <= _limit)
            {
                validPath.Add(node);
            }
        }

        return validPath;

	}

	public int GetDistance (Node nodeA, Node nodeB) {
		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);

		return 14 * distX + 10 * (distY - distX);
	}

    public void ResetCosts()
    {
        grid.ResetCosts();
    }

}
