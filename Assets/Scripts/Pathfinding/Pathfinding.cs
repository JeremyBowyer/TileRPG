using System;
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

    public bool CheckRange(Node startNode, Node targetNode, float _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable, bool ignoreMoveBlocks)
    {
        List<Node> _path = FindPath(startNode, targetNode, _limit, diag, ignoreOccupant, ignoreUnwalkable, false, ignoreMoveBlocks);

        return _path.Count > 0;
    }

    public List<Node> FindGeometricRange(Node startNode, float _limit)
    {
        List<Node> nodesInRange = new List<Node>();
        Vector3 flatStart = new Vector3(startNode.worldPosition.x, 0, startNode.worldPosition.z);

        foreach (Node node in grid.grid)
        {
            if (node == null)
                continue;

            float distance = Vector3.Distance(flatStart, new Vector3(node.worldPosition.x, 0, node.worldPosition.z));
            float heightDiff = Mathf.Round(startNode.worldPosition.y - node.worldPosition.y);
            distance -= heightDiff;

            if(distance <= _limit * grid.nodeDiameter)
                nodesInRange.Add(node);
        }

        return nodesInRange;
    }

    public List<Node> FindRange(Node startNode, float _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable, bool includeOrigin, bool ignoreMoveBlocks, float costModifier = 1f)
    {
        List<Node> nodesInRange = new List<Node>();
        //Node startNode = grid.NodeFromWorldPoint(startPos);

        // First pass. Are nodes even close enough to consider?
        foreach (Node node in grid.grid)
        {
            if (node == null)
                continue;
            float distance = GetDistance(startNode, node);
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
            List<Node> _path = FindPath(startNode, node, _limit, diag, ignoreOccupant, ignoreUnwalkable, includeOrigin, ignoreMoveBlocks, costModifier);
            if (_path.Contains(node))
            {
                validNodesInRange.Add(node);
            }
        }

        return validNodesInRange;

    }

    public List<Node> FindLine(Node startNode, Node endNode, float _limit, bool ignoreOccupant, bool ignoreUnwalkable)
    {
        List<Node> nodesInRange = new List<Node>();

        if (startNode.gridX != endNode.gridX && startNode.gridY != endNode.gridY)
            return nodesInRange;

        if(startNode.gridX == endNode.gridX)
        {
            int dist = Math.Abs(endNode.gridY - startNode.gridY);
            for(int i=0; i < dist+1; i++)
            {
                if (i >= _limit) return nodesInRange;
                int step = endNode.gridY > startNode.gridY ? startNode.gridY + i : startNode.gridY - i;
                nodesInRange.Add(grid.grid[startNode.gridX, step]);
            }
        } else if (startNode.gridY == endNode.gridY)
        {
            int dist = Math.Abs(endNode.gridX - startNode.gridX);
            for (int i = 0; i < dist+1; i++)
            {
                if (i >= _limit) return nodesInRange;
                int step = endNode.gridX > startNode.gridX ? startNode.gridX + i : startNode.gridX - i;
                nodesInRange.Add(grid.grid[step, startNode.gridY]);
            }
        }

        return nodesInRange;
    }

    public List<Node> CullNodes(List<Node> nodes, bool cullOccupied, bool cullUnwalkable)
    {
        // Meant to be fed an output from FindPath() or FindLine(), to remove invalid nodes.
        // This separation allows me to include invalid nodes in FindPath() and FindLine() for
        // movement/abilities that should be able to move THROUGH invalid nodes, just not land on them.

        List<Node> culledNodes = new List<Node>();

        foreach(Node node in nodes)
        {
            if (node.occupant != null && cullOccupied)
                continue;

            if (!node.IsWalkable && cullUnwalkable)
                continue;

            culledNodes.Add(node);
        }

        return culledNodes;
    }

	public List<Node> FindPath(Node startNode, Node targetNode, float _limit, bool diag, bool ignoreOccupant, bool ignoreUnwalkable, bool includeOrigin, bool ignoreMoveBlocks, float costModifier = 1f)
    {

        // ignoreOccupant and ignoreUnwalkable should be FALSE only if the movement/ability 
        // shouldn't be able to move THROUGH these nodes. If they can move through these nodes,
        // just not end on them, then keep those bools TRUE, and feed the resulting nodes to CullNodes()

        ResetCosts();

        Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);
		while (openSet.Count > 0) {
		
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add (currentNode);

            if (currentNode == targetNode) {
                return RetracePath(startNode, currentNode, _limit, includeOrigin);
            }

			foreach (Node neighbor in grid.GetNeighbors(currentNode, diag, ignoreOccupant, ignoreMoveBlocks)) {
				if ((!neighbor.IsWalkable && !ignoreUnwalkable) || closedSet.Contains(neighbor)) {
					continue;
                }

				float newMovementCostToNeighbor = currentNode.gCost + GetDistance (currentNode, neighbor, costModifier);
				if( newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
					neighbor.gCost = newMovementCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode, costModifier);
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

	List<Node> RetracePath(Node startNode, Node endNode, float _limit, bool includeOrigin) {

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

	public float GetDistance (Node nodeA, Node nodeB, float costModifier = 1f) {
		float distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
        float distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY)
			return (1.4f * distY * costModifier) + (1f * (distX - distY) * costModifier);

		return (1.4f * distX * costModifier) + (1f * (distY - distX) * costModifier);
	}

    public void ResetCosts()
    {
        grid.ResetCosts();
    }

}
