using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {

	// References
	//private BattleController bc;
	//private Pathfinding pathfinder;
    private Grid grid;
    //private GameObject apCost;
    public Node node;

    public GameObject occupant;
    public bool isWalkable
    {
        get
        {
            return LayerMask.LayerToName(gameObject.layer) != "Unwalkable";
        }
    }

	void Start() {
		//bc = GameObject.Find ("BattleController").GetComponent<BattleController>();
        //pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        //apCost = GameObject.Find("APCost");
        node = grid.NodeFromWorldPoint(transform.position);
    }

}
