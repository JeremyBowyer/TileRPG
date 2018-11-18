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
        base.Enter();
        StartCoroutine(gc.ZoomCamera(5f, 3f, 15f));
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        gc.EnableRBs(false);
        gc.protag.transform.position = new Vector3(gc.protag.transform.position.x, grid.FindHeightPoint(gc.protag.transform.position), gc.protag.transform.position.z);
        gc.protagStartPos = gc.protag.transform.position;

        gc.protag.transform.LookAt(new Vector3(gc.battleInitiator.transform.position.x, gc.protag.transform.position.y, gc.battleInitiator.transform.position.z));
        gc.currentCharacter = gc.protag;
        grid.CreateGrid();
        uiController.gameObject.SetActive(true);

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {

            Node node = gc.grid.FindNearestNode(enemy.transform.position);

            if (node != null)
            {
                enemy.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.backwardDirection, Vector3.up);
                enemy.GetComponent<Enemy>().Place(node.tile);
                enemy.GetComponent<Enemy>().InitBattle();
                enemy.GetComponent<Enemy>().statusIndicator.gameObject.SetActive(true);
                gc.enemies.Add(enemy);
            }
        }
        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);

        gc.protag.Place(protagNode.tile);
        gc.protag.InitBattle();
        yield return null;
        gc.ChangeState<SelectUnitState>();
    }
}