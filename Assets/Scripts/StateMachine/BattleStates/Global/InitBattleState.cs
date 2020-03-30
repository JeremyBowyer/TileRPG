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
    private KeepBattleRoom room;

    private bool surveyDone = false;
    private bool gridDone = false;
    private bool setupDone = false;

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
        room = args.room as KeepBattleRoom;

        // Start Coroutines
        StartCoroutine(grid.GenerateGrid(room, OnCreateGrid));
        StartCoroutine(SurveyBattleGrid());
        StartCoroutine(WaitForTasks());
    }

    public IEnumerator SurveyBattleGrid()
    {
        float duration = 1f;
        // Show Message
        superUI.ShowMinorMessage("Battle Start", room.enemies.Length * duration);

        float maxSize = Mathf.Max(new float[] { room.xSize, room.zSize });
        maxSize = Mathf.Max(new float[] { maxSize / 2.5f, 2f });
        bc.cameraRig.Zoom(2f, 1f, maxSize, 0.5f);

        foreach (GameObject enemyGO in room.enemies)
        {
            EnemyController enemy = enemyGO.GetComponent<EnemyController>();
            if (enemy.room != room)
                continue;

            bc.FollowTarget(enemy.transform);
            yield return new WaitForSeconds(duration);
        }

        surveyDone = true;
    }

    public void OnCreateGrid()
    {
        SetUpObjects();
        gridDone = true;
    }

    public void SetUpObjects()
    {
        //UIController.instance.SwitchTo("battle");
        //battleUI.ShowPanel("combat");

        // Re-position protag
        // Set up protag
        protag = bc.protag;
        protag.transform.position = new Vector3(protag.transform.position.x, grid.FindHeightPoint(protag.transform.position), protag.transform.position.z);
        bc.protagStartPos = protag.transform.position;
        protag.protagAgent.enabled = false;

        //protag.tile = frontNode.tile;
        protag.InitBattle();
        bc.unitsToPlace.Enqueue(protag.gameObject);

        bc.players.Add(protag.gameObject);
        bc.characters.Add(protag.gameObject);

        // Place party members on protag tile, to be moved in next state
        foreach (PartyMember member in PersistentObjects.party.GetMembers())
        {
            if (member == protag.character)
                continue;
            //member.controller.tile = frontNode.tile;
            member.controller.transform.position = bc.protagStartPos;
            member.controller.InitController();
            bc.players.Add(member.controller.gameObject);
            bc.characters.Add(member.controller.gameObject);
            bc.unitsToPlace.Enqueue(member.controller.gameObject);
        }

        // Setup Nearby Enemies
        foreach (GameObject enemyGO in room.enemies)
        {
            EnemyController enemy = enemyGO.GetComponent<EnemyController>();
            if (enemy.room != room)
                continue;

            Node node = bc.grid.FindNearestNode(enemyGO.transform.position, lowestDistance: 1f, ignoreOccupant: false);
            if (node != null)
            {
                enemy = enemyGO.GetComponent<EnemyController>();
                enemyGO.gameObject.transform.rotation = Quaternion.LookRotation(Grid.backwardDirection, Vector3.up);
                enemy.Place(node.tile);
                enemy.InitBattle();
                BaseAI enemyAI = enemyGO.GetComponent<BaseAI>();
                enemyAI.Init();
                bc.enemies.Add(enemyGO);
                bc.characters.Add(enemyGO);
                room.enemyGroup.AddMember(enemy.character);
            }
        }

        // Add delegates for units in battle
        foreach (GameObject character in bc.characters)
        {
            bc.onUnitChange += character.GetComponent<CharController>().OnUnitChange;
            bc.onRoundChange += character.GetComponent<CharController>().OnRoundChange;
        }
        bc.onUnitChange += bc.turnQueue.UpdateQueue;
        bc.protag.gameObject.transform.localScale = Vector3.zero;

        // Initiate round
        //bc.rc.InitRound(bc.characters);

        setupDone = true;
    }

    public IEnumerator WaitForTasks()
    {
        while (!gridDone || !setupDone || !surveyDone)
        {
            yield return null;
        }

        bc.EnableRBs(false);
        battleUI.gameObject.SetActive(true);
        UIController.instance.SwitchTo("battle");
        battleUI.ShowPanel("combat");

        // Instantiate Turn Entries
        foreach (GameObject go in bc.characters)
        {
            CharController controller = go.GetComponent<CharController>();
            bc.turnQueue.InstantiateEntry(controller);
        }

        InTransition = false;
        bc.ChangeState<PlaceUnitsState>();
    }

    protected override void OnKeyDown(object sender, InfoEventArgs<KeyCode> e)
    {
        if (e.info == KeyCode.V)
            battleUI.ShowCharUIs(true);
    }

    protected override void OnKeyUp(object sender, InfoEventArgs<KeyCode> e)
    {
        if (e.info == KeyCode.V)
            battleUI.ShowCharUIs(false);
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

}