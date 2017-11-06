using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler {

	// References
	private BattleMaster bm;
	private Pathfinding pathfinder;
    private Grid grid;
    //private GameObject apCostHolder;
    private GameObject apCost;
    private Node node;

    public GameObject occupant;
    private bool isWalkable;

	void Start() {
		bm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<BattleMaster>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        //apCostHolder = GameObject.Find("APCostHolder");
        apCost = GameObject.Find("APCost");
        node = grid.NodeFromWorldPoint(transform.position);
        isWalkable = LayerMask.LayerToName(gameObject.layer) != "Unwalkable";
    }

    private void OnMouseExit()
    {
        grid.path = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!bm.paused)
            {
                if (bm.currentState == BattleMaster.BattleState.PLAYERMOVE)
                {
                    bm.curPlayer.PlayerMove(this);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(bm.currentState == BattleMaster.BattleState.PLAYERMOVE)
        {
            if (!isWalkable)
            {
                grid.path = null;
            }
            else if (!bm.paused)
            {
                grid.path = pathfinder.FindPath(bm.curPlayer.gameObject.transform.position, transform.position, 999 /*bm.curPlayer.GetComponent<Player>().stats.moveRange*/);
                apCost.GetComponent<Text>().text = node.gCost.ToString() + " / " + bm.curPlayer.stats.moveRange;

                if (node.gCost > bm.curPlayer.stats.moveRange)
                {
                    apCost.GetComponent<Text>().color = Color.red;
                }
                else
                {
                    apCost.GetComponent<Text>().color = Color.green;
                }
            }
        }
    }
}
