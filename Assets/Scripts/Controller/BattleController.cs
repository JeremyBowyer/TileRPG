using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : GameController
{
    // References
    public RoundController rc;
    public ProjectileValidationController pvc;
    public TurnQueueController turnQueue;
    public BattleUIController battleUiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public LineRenderer lineRenderer;
    public GameObject selectedCharacterAura;

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
            if (battleUiController != null)
                battleUiController.UpdateStats();
        }
    }
    public Tile currentTile;
    public Queue<GameObject> unitsToPlace; // Queue of party members to be placed on the grid
    public EnemyController battleInitiator;
    public Vector3 protagStartPos = new Vector3(0, 0, 0);

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
        unitsToPlace = new Queue<GameObject>();

        //cameraRig.FollowTarget = protag.transform;
        //cameraTarget = protag.transform;
        ChangeState<IdleState>();
    }

    public void Init()
    {
        ChangeState<InitBattleState>();
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
        cameraTarget = character.transform;

        selectedCharacterAura.gameObject.transform.SetParent(CurrentCharacter.gameObject.transform);
        selectedCharacterAura.transform.localPosition = new Vector3(0, 0, 0);
        selectedCharacterAura.SetActive(true);
        Projector projector = selectedCharacterAura.GetComponent<Projector>();
        projector.material = new Material(projector.material);

        if (character is EnemyController)
            projector.material.SetColor("_RampColorTint", CustomColors.Hostile);

        if (character is PlayerController)
            projector.material.SetColor("_RampColorTint", CustomColors.PlayerUI);



        if (onUnitChange != null)
            onUnitChange(character);
    }

    public void OnUnitDeath(CharController character)
    {
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
        CurrentCharacter = null;
    }

}
