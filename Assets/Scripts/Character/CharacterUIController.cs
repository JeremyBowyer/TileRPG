using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIController : MonoBehaviour
{

    private Material material;
    private Color normalColor;
    private Color highlightcolor;
    private bool touching;
    private Grid grid;

    private BattleMaster bm;

    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<BattleMaster>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();


        material = GetComponent<MeshRenderer>().material;
        normalColor = material.color;
        highlightcolor = new Color(
            normalColor.r * 2.5f,
            normalColor.g * 0.5f,
            normalColor.b * 0.5f
            );

    }

    void Update()
    {
        if (touching)
        {
            material.color = highlightcolor;
        }

    }

    private void OnMouseDown()
    {
        if (bm.currentState == BattleMaster.BattleState.PLAYERATTACK && grid.range.Contains(grid.NodeFromWorldPoint(transform.position)))
        {
            bm.curPlayer.PlayerAttack(this.GetComponent<Player>(), bm.curPlayer.curAbility);
        }
    }

    private void OnMouseEnter()
    {
        if (bm.currentState == BattleMaster.BattleState.PLAYERATTACK && grid.range.Contains(grid.NodeFromWorldPoint(transform.position)))
        {
            touching = true;
        }
    }

    private void OnMouseExit()
    {
        touching = false;
        material.color = normalColor;
    }

}
