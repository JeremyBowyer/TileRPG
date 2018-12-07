using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class InitBattleState : BattleState
{
    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    public override bool isInterruptable
    {
        get { return false; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.protag.animParamController.SetBool("idle");
        StartCoroutine(grid.CreateGrid(OnCreateGrid));
        StartCoroutine(gc.ZoomCamera(5f, 3f, 15f));
    }
      
    public void OnCreateGrid()
    {
        gc.EnableRBs(false);
        gc.protag.transform.position = new Vector3(gc.protag.transform.position.x, grid.FindHeightPoint(gc.protag.transform.position), gc.protag.transform.position.z);
        gc.protagStartPos = gc.protag.transform.position;

        gc.protag.transform.LookAt(new Vector3(gc.battleInitiator.transform.position.x, gc.protag.transform.position.y, gc.battleInitiator.transform.position.z));
        uiController.gameObject.SetActive(true);

        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);
        gc.protag.Place(protagNode.tile);
        gc.protag.InitBattle();

        foreach (GameObject player in gc.players)
        {
            gc.battleCharacters.Add(player);
        }

        foreach (GameObject enemyGO in GameObject.FindGameObjectsWithTag("Enemy"))
        {

            Node node = gc.grid.FindNearestNode(enemyGO.transform.position);

            if (node != null)
            {
                Enemy enemy = enemyGO.GetComponent<Enemy>();
                enemyGO.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.backwardDirection, Vector3.up);
                enemy.Place(node.tile);
                enemy.InitBattle();
                BaseAI enemyAI = enemyGO.GetComponent<BaseAI>();
                enemyAI.Init();
                gc.battleEnemies.Add(enemyGO);
                gc.battleCharacters.Add(enemyGO);
            }
        }

        foreach (GameObject character in gc.battleCharacters)
        {
            gc.onUnitChange += character.GetComponent<Character>().OnTurnEnd;
        }

        gc.currentCharacter = gc.battleEnemies[0].GetComponent<Enemy>();

        inTransition = false;
        gc.ChangeState<SelectUnitState>();
    }
}