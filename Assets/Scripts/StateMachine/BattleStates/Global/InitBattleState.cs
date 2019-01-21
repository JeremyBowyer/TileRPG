using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class InitBattleState : BattleState
{
    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;
    private ProtagonistController protag;

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
            typeof(PlaceUnitsState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.protag.animParamController.SetBool("idle");

        // Re-position protag
        gc.protag.transform.position = new Vector3(gc.protag.transform.position.x, grid.FindHeightPoint(gc.protag.transform.position), gc.protag.transform.position.z);
        gc.protag.transform.LookAt(new Vector3(gc.battleInitiator.transform.position.x, gc.protag.transform.position.y, gc.battleInitiator.transform.position.z));
        gc.protagStartPos = gc.protag.transform.position;

        // Show Message
        superUiController.ShowMessage("Battle Start", 2f);

        // Start Coroutines
        StartCoroutine(grid.CreateGrid(OnCreateGrid));
        StartCoroutine(gc.cameraRig.ZoomCamera(5f, 3f, 15f));
    }
      
    public void OnCreateGrid()
    {
        gc.EnableRBs(false);
        battleUiController.gameObject.SetActive(true);
        worldUiController.gameObject.SetActive(false);

        // Place protag on tile
        Node protagNode = gc.grid.FindNearestNode(gc.protag.transform.position);
        gc.battleCharacters.Add(gc.protag.gameObject);
        gc.protag.Place(protagNode.tile);
        gc.protag.InitBattle();

        // Place party members on protag tile, to be moved in next phase
        protag = gc.protag;
        foreach (PartyMember member in protag.partyMembers)
        {
            member.controller.Place(protagNode.tile);
            gc.battleCharacters.Add(member.controller.gameObject);
            gc.unitsToPlace.Enqueue(member.controller.gameObject);
        }
        gc.unitsToPlace.Enqueue(protag.gameObject);

        // Setup Nearby Enemies
        foreach (GameObject enemyGO in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Node node = gc.grid.FindNearestNode(enemyGO.transform.position);

            if (node != null)
            {
                EnemyController enemy = enemyGO.GetComponent<EnemyController>();
                enemyGO.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.backwardDirection, Vector3.up);
                enemy.Place(node.tile);
                enemy.InitBattle();
                BaseAI enemyAI = enemyGO.GetComponent<BaseAI>();
                enemyAI.Init();
                gc.battleEnemies.Add(enemyGO);
                gc.battleCharacters.Add(enemyGO);
            }
        }

        // Add delegates for units in battle
        foreach (GameObject character in gc.battleCharacters)
        {
            gc.onUnitChange += character.GetComponent<CharController>().OnTurnEnd;
        }

        gc.protag.gameObject.transform.localScale = Vector3.zero;

        inTransition = false;
        gc.ChangeState<PlaceUnitsState>();
    }

}