using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InitBattleState : BattleState
{
    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    public override void Enter()
    {
        isInterruptable = false;
        inTransition = true;
        StartCoroutine(gc.ZoomCamera(5f, 3f, 15f));
        base.Enter();

        gc.EnableRBs(false);
        gc.protag.transform.position = new Vector3(gc.protag.transform.position.x, grid.FindHeightPoint(gc.protag.transform.position), gc.protag.transform.position.z);
        gc.protagStartPos = gc.protag.transform.position;

        gc.protag.transform.LookAt(new Vector3(gc.battleInitiator.transform.position.x, gc.protag.transform.position.y, gc.battleInitiator.transform.position.z));
        gc.currentCharacter = gc.protag;
        grid.CreateGrid();
        uiController.gameObject.SetActive(true);

        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);
        gc.protag.Place(protagNode.tile);
        gc.protag.InitBattle();

        foreach (GameObject enemyGO in GameObject.FindGameObjectsWithTag("Enemy"))
        {

            Node node = gc.grid.FindNearestNode(enemyGO.transform.position);

            if (node != null)
            {
                Enemy enemy = enemyGO.GetComponent<Enemy>();
                enemyGO.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.backwardDirection, Vector3.up);
                enemy.Place(node.tile);
                enemy.InitBattle();
                enemy.statusIndicator.gameObject.SetActive(true);
                BaseAI enemyAI = enemyGO.GetComponent<BaseAI>();
                enemyAI.Init();
                gc.battleEnemies.Add(enemyGO);
                gc.battleCharacters.Add(enemyGO);
            }
        }

        foreach (GameObject player in gc.players)
        {
            gc.battleCharacters.Add(player);
        }

        foreach (GameObject character in gc.battleCharacters)
        {
            gc.onUnitChange += character.GetComponent<Character>().OnTurnEnd;
        }

        inTransition = false;
        gc.ChangeState<SelectUnitState>();
    }
}