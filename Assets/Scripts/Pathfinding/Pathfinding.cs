using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	Grid grid;

	void Awake(){
		grid = GetComponent<Grid> ();
	}

	void Update() {

	}

    public bool CheckRange(Vector3 startPos, Vector3 targetPos, int _limit)
    {
        List<Node> _path = FindPath(startPos, targetPos, _limit);

        return _path.Count > 0;
    }

    public void FindRange(Vector3 startPos, int _limit)
    {
        List<Node> nodesInRange = new List<Node>();
        Node startNode = grid.NodeFromWorldPoint(startPos);

        foreach (Node node in grid.grid)
        {
            int distance = GetDistance(startNode, node);
            if(distance <= _limit)
            {
                nodesInRange.Add(node);
            }
        }

        List<Node> validNodesInRange = new List<Node>();
        for (int i = 0; i < nodesInRange.Count; i++)
        {
            Node node = nodesInRange[i];
            List<Node> _path = FindPath(startPos, node.worldPosition, _limit);
            if (_path.Contains(node))
            {
                validNodesInRange.Add(node);
            }
        }
        grid.range = validNodesInRange;

    }

	public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, int _limit) {

        grid.ResetCosts();

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

        Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);

		while (openSet.Count > 0) {
		
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add (currentNode);

			if (currentNode == targetNode) {
				return RetracePath(startNode, currentNode, _limit);
            }

			foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
				if (!neighbor.walkable || closedSet.Contains(neighbor)) {
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

	List<Node> RetracePath(Node startNode, Node endNode, int _limit) {
		List<Node> _path = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			_path.Add (currentNode);
			currentNode = currentNode.parent;
		}

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

	int GetDistance (Node nodeA, Node nodeB) {
		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);

		return 14 * distX + 10 * (distY - distX);
	}

}
