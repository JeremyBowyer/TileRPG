using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public Tile tile = null;
	public bool walkable
    {
        get
        {
            return tile.isWalkable;
        }
    }
    public GameObject occupant
    {
        get
        {
            return tile.occupant;
        }
    }
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public int fCost { get { return gCost + hCost; } }
	int heapIndex;

	public Node parent;

	public Node(Vector3 _worldPos, int _gridX, int _gridY) {
		worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo (nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (nodeToCompare.hCost);
		}

		return -compare;
	}

}
