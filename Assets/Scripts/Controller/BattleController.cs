using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleController : GameController
{
    // References
    public RoundController rc;
    public ProjectileValidationController pvc;
    public TurnQueueController turnQueue;
    public BattleUIController battleUI { get { return lc.uiController.battleUI; } }
    public Grid grid;
    public Pathfinding pathfinder;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public LineRenderer lineRenderer;
    public GameObject selectionCircle;
    public LevelController lc;
    public BSPBattleRoom battleRoom;

    // Variables
    private CharController currentCharacter;
    public CharController CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            if (battleUI != null)
                battleUI.UpdateStats();
        }
    }
    public Tile currentTile;
    public Queue<GameObject> unitsToPlace; // Queue of party members to be placed on the grid
    public Vector3 protagStartPos = new Vector3(0, 0, 0);
    private bool inBattle;
    public bool InBattle
    {
        get { return inBattle; }
        set { inBattle = value; }
    }

    // Delegates
    public delegate void OnUnitChange(CharController character);
    public OnUnitChange onUnitChange;

    public delegate void OnRoundChange();
    public OnRoundChange onRoundChange;

    void Start()
    {
        // Assign references
        rc = new RoundController(this);
        pvc = new ProjectileValidationController(this);
        lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        unitsToPlace = new Queue<GameObject>();

        //cameraRig.FollowTarget = protag.transform;
        //cameraTarget = protag.transform;
        ChangeState<IdleState>();
    }

    public void Init(BSPBattleRoom _room)
    {
        battleRoom = _room;
        StateArgs args = new StateArgs()
        {
            room = _room
        };
        InBattle = true;
        ChangeState<InitBattleState>(args);
    }

    public void NextPlayer()
    {
        // If there is a currenct character, invoke relevant methods
        if (CurrentCharacter != null)
        {
            turnQueue.HideEntry(CurrentCharacter.turnEntry);
            rc.CharacterTurnEnd(CurrentCharacter);
            CurrentCharacter.OnTurnEnd();
        }

        // If no characters left in round, initiate new round
        if (rc.roundChars.Count < 1)
        {
            rc.InitRound(characters);
        }

        // Determine turn order, to find next character
        rc.DetermineTurnOrder();
        ChangePlayer(rc.roundChars[0]);

        /*
        if (CurrentCharacter == null)
        {
            ChangePlayer(characters[0].GetComponent<CharController>());
            return;
        }
        int index = characters.IndexOf(CurrentCharacter.gameObject);
        index = (index + 2 > characters.Count) ? 0 : index + 1;
        ChangePlayer(characters[index].GetComponent<CharController>());
        */
    }

    public void ChangePlayer(CharController character)
    {
        CurrentCharacter = character;
        lc.cameraTarget = character.transform;

        selectionCircle.gameObject.transform.SetParent(CurrentCharacter.gameObject.transform);
        selectionCircle.transform.localPosition = new Vector3(0, 0, 0);
        selectionCircle.SetActive(true);
        Projector projector = selectionCircle.GetComponent<Projector>();
        projector.material = new Material(projector.material);

        if (character is EnemyController)
            projector.material.SetColor("_Color", CustomColors.Hostile);

        if (character is PlayerController)
            projector.material.SetColor("_Color", CustomColors.PlayerUI);



        if (onUnitChange != null)
            onUnitChange(character);
    }

    public void OnUnitDeath(CharController character)
    {
        characters.Remove(character.gameObject);
        lc.characters.Remove(character.gameObject);
        rc.RemoveCharacter(character);
        turnQueue.HideEntry(character);
        turnQueue.UpdateQueue(character);
        CheckEndCondition();
    }

    public void CheckEndCondition()
    {
        if (enemies.Count == 0 && !IsInQueue(typeof(VictorySequence)))
        {
            ChangeState<VictorySequence>();
        }

        if (players.Count == 0)
        {
            ChangeState<LossSequence>();
        }
    }

    public void TerminateBattle()
    {
        lc.bspController.ShowAllRooms();
        grid.ClearGrid();
        characters.Clear();
        selectionCircle.gameObject.transform.parent = null;
        selectionCircle.SetActive(false);
        turnQueue.EndBattle();
        // Set protag as camera target
        lc.cameraTarget = protag.transform;

        // Place protag on starting spot
        NavMeshAgent protagAgent = protag.GetComponent<NavMeshAgent>();
        //protagAgent.Warp(lc.startingPos);
        protagAgent.Warp(protagAgent.transform.position);
        battleRoom.completed = true;
        battleRoom = null;
        InBattle = false;
        CurrentCharacter = null;
    }

}
