using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : GameController
{
    // References
    public BattleUIController battleUiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public LineRenderer lineRenderer;

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

    void Start()
    {
        // Assign references
        lineRenderer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        unitsToPlace = new Queue<GameObject>();

        cameraRig.FollowTarget = protag.transform;
        cameraTarget = protag.transform;

        ChangeState<InitBattleState>();

    }

    public void NextPlayer()
    {
        if (CurrentCharacter == null)
        {
            ChangePlayer(characters[0].GetComponent<CharController>());
            return;
        }
        int index = characters.IndexOf(CurrentCharacter.gameObject);
        index = (index + 2 > characters.Count) ? 0 : index + 1;
        ChangePlayer(characters[index].GetComponent<CharController>());
    }

    public void ChangePlayer(CharController character)
    {
        CurrentCharacter = character;
        cameraTarget = character.transform;
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
