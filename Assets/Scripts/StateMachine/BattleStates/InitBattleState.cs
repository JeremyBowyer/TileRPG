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
        foreach(GameObject character in gc.characters)
        {
            Rigidbody rb = character.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            
        }

        gc.protag.transform.LookAt(new Vector3(gc.battleInitiator.transform.position.x, gc.protag.transform.position.y, gc.battleInitiator.transform.position.z));
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
            }
        }
        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);
        gc.currentCharacter.Place(protagNode.tile);
        yield return null;
        gc.ChangeState<SelectUnitState>();
    }
}