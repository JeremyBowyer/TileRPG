using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {

	// References
	//private BattleController bc;
	//private Pathfinding pathfinder;
    public Grid grid;
    //private GameObject apCost;
    public Node node;
    public Vector3 worldPosition { get { return node.worldPosition;  } }
    public float worldX { get { return node.worldPosition.x; } }
    public float worldY { get { return node.worldPosition.y; } }

    public GameObject occupant;
    public bool isWalkable
    {
        get
        {
            return LayerMask.LayerToName(gameObject.layer) != "Unwalkable";
        }
    }

	void Start() {
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
    }

}
