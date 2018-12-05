using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : StateMachine
{

    // References
    public CameraController cameraRig;
    public Camera _camera;
    public CameraRotate _cameraController;
    public BattleUIController uiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;

    // Variables
    public Character currentCharacter;
    public Player protag;
    public Tile currentTile;
    public List<GameObject> players; // All players on map
    public List<GameObject> battleEnemies; // Enemies in battle
    public List<GameObject> worldEnemies; // All enemies on map
    public List<GameObject> characters; // All enemies and players on map
    public List<GameObject> battleCharacters; // Character in battle
    public Enemy battleInitiator;
    public Vector3 protagStartPos = new Vector3(0, 0, 0);

    // Delegates
    public delegate void OnUnitChange(Character character);
    public OnUnitChange onUnitChange;

    void Start()
    {
        // Assign references
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<Player>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        _cameraController = GameObject.Find("Camera").GetComponent<CameraRotate>();

        // Find Enemies and Players, add them to lists
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            worldEnemies.Add(enemy);
            characters.Add(enemy);
        }

        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
            characters.Add(player);
        }
        players.Add(protag.gameObject);
        characters.Add(protag.gameObject);

        // Check for conditions
        if (uiController == null)
            Debug.LogError("UIController not assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("StatusIndicator not assigned to " + gameObject.name);

        // Set delegate for camera
        //onUnitChange += cameraRig.NewTarget;

        ChangeState<WorldExploreState>();
    }

    public void NextPlayer()
    {
        int index = battleCharacters.IndexOf(currentCharacter.gameObject);
        index = (index+2 > battleCharacters.Count) ? 0 : index + 1;
        ChangePlayer(battleCharacters[index].GetComponent<Character>());
    }

    public void ChangePlayer(Character character)
    {
        currentCharacter = character;
        if (onUnitChange != null)
            onUnitChange(character);
    }

    public void OnUnitDeath(Character character)
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

    public IEnumerator ZoomCamera(float targetSize, float minSize, float maxSize)
    {
        float currentSize = _camera.orthographicSize;
        float speed = 2f;
        _cameraController.minSize = minSize;
        _cameraController.maxSize = maxSize;

        float min = Mathf.Min(new float[] { currentSize, targetSize });
        float max = Mathf.Max(new float[] { currentSize, targetSize });

        while (!Mathf.Approximately(_camera.orthographicSize, targetSize))
        {
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + (targetSize - currentSize) * Time.deltaTime * speed, min, max);
            yield return new WaitForEndOfFrame();
        }
        yield break;
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