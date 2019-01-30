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
        bc.protag.animParamController.SetBool("idle");

        // Show Message
        superUiController.ShowMessage("Battle Start", 2f);

        // Start Coroutines
        StartCoroutine(grid.GenerateGrid(OnCreateGrid));
        StartCoroutine(bc.cameraRig.ZoomCamera(5f, 3f, 15f));
    }
      
    public void OnCreateGrid()
    {
        bc.EnableRBs(false);
        battleUiController.gameObject.SetActive(true);

        // Re-position protag
        // Set up protag
        protag = bc.protag;
        protag.transform.position = new Vector3(protag.transform.position.x, grid.FindHeightPoint(protag.transform.position), protag.transform.position.z);
        bc.protagStartPos = protag.transform.position;

        // Place protag
        Node protagNode = bc.grid.FindNearestNode(protag.transform.position);
        protag.Place(protagNode.tile);
        protag.InitBattle();
        bc.unitsToPlace.Enqueue(protag.gameObject);

        protag.InstantiatePartyMembers();

        bc.players.Add(protag.gameObject);
        bc.characters.Add(protag.gameObject);
        // Place party members on protag tile, to be moved in next state
        foreach (PartyMember member in protag.partyMembers)
        {
            member.controller.Place(protagNode.tile);
            bc.players.Add(member.controller.gameObject);
            bc.characters.Add(member.controller.gameObject);
            bc.unitsToPlace.Enqueue(member.controller.gameObject);
        }

        // Setup Nearby Enemies
        foreach (GameObject enemyGO in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Node node = bc.grid.FindNearestNode(enemyGO.transform.position);

            if (node != null)
            {
                EnemyController enemy = enemyGO.GetComponent<EnemyController>();
                enemyGO.gameObject.transform.rotation = Quaternion.LookRotation(bc.grid.backwardDirection, Vector3.up);
                enemy.Place(node.tile);
                enemy.InitBattle();
                BaseAI enemyAI = enemyGO.GetComponent<BaseAI>();
                enemyAI.Init();
                bc.enemies.Add(enemyGO);
                bc.characters.Add(enemyGO);
            }
        }

        // Add delegates for units in battle
        foreach (GameObject character in bc.characters)
        {
            bc.onUnitChange += character.GetComponent<CharController>().OnTurnEnd;
        }

        bc.protag.gameObject.transform.localScale = Vector3.zero;

        inTransition = false;
        bc.ChangeState<PlaceUnitsState>();
    }

}