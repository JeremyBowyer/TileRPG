using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : StateMachine
{

    // References
    public CameraController cameraRig;
    public Camera _camera;
    public BattleUIController battleUiController;
    public WorldUIController worldUiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public WorldMenuPanelController worldMenuPanelController;
    public SuperUIController superUiController;
    public GameObject movementCursor;
    public LineRenderer lineRenderer;

    // Variables
    public CharController currentCharacter;
    public ProtagonistController protag;
    public Tile currentTile;
    public List<GameObject> players; // All players on map
    public List<GameObject> battleEnemies; // Enemies in battle
    public List<GameObject> worldEnemies; // All enemies on map
    public List<GameObject> characters; // All enemies and players on map
    public List<GameObject> battleCharacters; // Character in battle
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

        ChangeState<InitLevelState>();
    }

    public void PauseGame()
    {
        foreach (GameObject character in characters)
        {
            CharController controller = character.GetComponent<CharController>();
            controller.Pause();
        }
    }

    public void ResumeGame()
    {
        foreach (GameObject character in characters)
        {
            CharController controller = character.GetComponent<CharController>();
            controller.Resume();
        }
    }

    public void NextPlayer()
    {
        if(currentCharacter == null)
        {
            ChangePlayer(battleCharacters[0].GetComponent<CharController>());
            return;
        }
        int index = battleCharacters.IndexOf(currentCharacter.gameObject);
        index = (index+2 > battleCharacters.Count) ? 0 : index + 1;
        ChangePlayer(battleCharacters[index].GetComponent<CharController>());
    }

    public void ChangePlayer(CharController character)
    {
        currentCharacter = character;
        if (onUnitChange != null)
            onUnitChange(character);
    }

    public void OnUnitDeath(CharController character)
    {
        CheckEndCondition();
    }

    public void CheckEndCondition()
    {
        if (battleEnemies.Count == 0 && !IsInQueue(typeof(VictorySequence)))
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
        currentCharacter = null;
    }

    public void EnableRBs(bool enabled)
    {
        foreach (GameObject character in characters)
        {
            Rigidbody rb = character.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = !enabled;
                rb.useGravity = enabled;
            }

        }
    }

}