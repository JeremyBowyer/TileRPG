using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleController : GameController
{
    // References
    [HideInInspector]
    public RoundController rc;
    [HideInInspector]
    public ProjectileValidationController pvc;
    [HideInInspector]
    public TurnQueueController turnQueue;
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Pathfinding pathfinder;
    [HideInInspector]
    public RewardController rewardController;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public LineRenderer lineRenderer;
    public GameObject selectionCircle;
    public LevelController lc;
    public KeepBattleRoom battleRoom;
    public BattleUIController battleUI { get { return lc.uiController.battleUI; } }

    public static BattleController instance;

    // Directions
    public static Vector3 rightDirection { get { return LevelController.rightDirection; } }
    public static Vector3 leftDirection { get { return LevelController.leftDirection; } }
    public static Vector3 forwardDirection { get { return LevelController.forwardDirection; } }
    public static Vector3 backwardDirection { get { return LevelController.backwardDirection; } }
    public static Vector3 forwardLeftDirection { get { return LevelController.forwardLeftDirection; } }
    public static Vector3 forwardRightDirection { get { return LevelController.forwardRightDirection; } }
    public static Vector3 backwardLeftDirection { get { return LevelController.backwardLeftDirection; } }
    public static Vector3 backwardRightDirection { get { return LevelController.backwardRightDirection; } }

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
                battleUI.LoadCurrentStats(null, currentCharacter);
        }
    }

    private CharController targetCharacter;
    public CharController TargetCharacter
    {
        get
        {
            return targetCharacter;
        }
        set
        {
            targetCharacter = value;
            if (battleUI != null)
            {
                if (targetCharacter == null)
                    battleUI.UnloadTargetStats();
                else
                    battleUI.LoadTargetStats(targetCharacter);
            }
        }
    }

    [HideInInspector]
    public Tile currentTile;
    [HideInInspector]
    public Queue<GameObject> unitsToPlace; // Queue of party members to be placed on the grid
    [HideInInspector]
    public Queue<KeyValuePair<CharController, Damage[]>> damageQueue;
    public Vector3 protagStartPos = new Vector3(0, 0, 0);
    private bool inBattle;
    public bool InBattle
    {
        get { return inBattle; }
        set { inBattle = value; }
    }

    // Delegates
    public delegate void OnUnitChange(CharController previousCharacter, CharController currentCharacter);
    public OnUnitChange onUnitChange;

    public delegate void OnUnitDie(CharController character, Damage damage);
    public OnUnitDie onUnitDeath;

    public delegate void OnRoundChange();
    public OnRoundChange onRoundChange;

    private void Awake()
    {
        instance = this;
        AssignReferences();
    }

    void Start()
    {
        ChangeState<IdleState>();
    }

    public override void AssignReferences()
    {
        base.AssignReferences();

        rewardController = new RewardController();
        rc = new RoundController();
        pvc = new ProjectileValidationController(this);
        lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        unitsToPlace = new Queue<GameObject>();
        damageQueue = new Queue<KeyValuePair<CharController, Damage[]>>();
    }

    public void InitBattle(KeepBattleRoom _room)
    {
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        battleRoom = _room;
        battleRoom.GetComponent<BoxCollider>().enabled = false;
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
    }

    public void ChangePlayer(CharController character)
    {
        onUnitChange?.Invoke(CurrentCharacter, character);
        CurrentCharacter = character;
        FollowTarget(character.transform);

        selectionCircle.gameObject.transform.SetParent(CurrentCharacter.gameObject.transform);
        selectionCircle.transform.localPosition = new Vector3(0, 0, 0);
        selectionCircle.SetActive(true);
        Projector projector = selectionCircle.GetComponent<Projector>();
        projector.material = new Material(projector.material);

        if (character is EnemyController)
            projector.material.SetColor("_Color", CustomColors.Hostile);

        if (character is PlayerController)
            projector.material.SetColor("_Color", CustomColors.PlayerUI);
    }

    public void OnUnitDeath(CharController character, Damage damage)
    {
        characters.Remove(character.gameObject);
        lc.characters.Remove(character.gameObject);
        rc.RemoveCharacter(character);
        turnQueue.HideEntry(character);
        turnQueue.UpdateQueue(null, character);
        onUnitDeath?.Invoke(character, damage);

        if (character is EnemyController)
        {
            enemies.Remove(character.gameObject);
            lc.enemies.Remove(character.gameObject);
        }

        if (CurrentCharacter == character)
            CurrentCharacter = null;

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
        //lc.bspController.ShowAllRooms();
        grid.ClearGrid();
        characters.Clear();
        selectionCircle.gameObject.transform.parent = null;
        selectionCircle.SetActive(false);
        turnQueue.EndBattle();
        battleRoom.GetComponent<BoxCollider>().enabled = true;

        // Set protag as camera target
        FollowTarget(protag.transform);
        NavMeshAgent protagAgent = protag.GetComponent<NavMeshAgent>();
        protagAgent.Warp(protagAgent.transform.position);
        battleRoom.completed = true;
        battleRoom = null;
        InBattle = false;
        CurrentCharacter = null;
    }

}
