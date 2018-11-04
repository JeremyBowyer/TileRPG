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
        StartCoroutine(gc.ZoomCamera(5f));
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        gc.protag.transform.LookAt(gc.battleInitiator.transform);
        gc.currentCharacter = gc.protag;
        grid.CreateGrid();
        uiController.gameObject.SetActive(true);
        gc.currentCharacter.statusIndicator.gameObject.SetActive(true);

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {

            Node node = gc.grid.FindNearestNode(enemy.transform.position);

            if (node != null)
            {
                enemy.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.backwardDirection, Vector3.up);
                enemy.GetComponent<Enemy>().Place(node.tile);
                gc.enemies.Add(enemy);
                gc.characters.Add(enemy.GetComponent<Enemy>());
            }
        }

        gc.players.Add(gc.protag.gameObject);
        gc.characters.Add(gc.protag.GetComponent<Player>());
        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);
        gc.currentCharacter.Place(protagNode.tile);

        yield return null;
        gc.ChangeState<SelectUnitState>();
    }
}