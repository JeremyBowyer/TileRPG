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
    private BSPRoom room;
    public override bool IsInterruptible
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
        InTransition = true;
        base.Enter();
        bc.protag.animParamController.SetBool("idle");
        UserInputController.AddLayer(LayerMask.NameToLayer("Character"));

        // Get args
        room = args.room;
        // Show Message
        superUI.ShowMessage("Battle Start", 2f);

        // Start Coroutines
        StartCoroutine(grid.GenerateGrid(room, OnCreateGrid));
        bc.cameraRig.Zoom(5f, 2f, 5f);
    }

    public void OnCreateGrid()
    {
        bc.EnableRBs(false);
        battleUI.gameObject.SetActive(true);

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
            EnemyController enemy = enemyGO.GetComponent<EnemyController>();
            if (enemy.room != room)
                continue;

            Node node = bc.grid.FindNearestNode(enemyGO.transform.position, lowestDistance: 1f, ignoreOccupant: false);
            if (node != null)
            {
                enemy = enemyGO.GetComponent<EnemyController>();
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
        bc.onUnitChange += bc.turnQueue.UpdateQueue;
        bc.protag.gameObject.transform.localScale = Vector3.zero;

        // Initiate round
        bc.rc.InitRound(bc.characters);

        // Instantiate Turn Entries
        foreach (GameObject go in bc.characters)
        {
            CharController controller = go.GetComponent<CharController>();
            bc.turnQueue.InstantiateEntry(controller);
        }

        InTransition = false;
        bc.ChangeState<PlaceUnitsState>();
    }

    public override void Exit()
    {
        base.Exit();
        AddListeners();
    }

    protected override void OnDestroy()
    {
        //RemoveListeners();
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        CharController target = e.info.gameObject.GetComponent<CharController>();

        if (target != null)
            events.LoadTargetCharacter(target);
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        events.UnloadTargetCharacter();
    }

}