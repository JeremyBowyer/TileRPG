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
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public Text playerName;

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
    public delegate void OnUnitChange();
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
        onUnitChange += cameraRig.NewTarget;

        ChangeState<WorldExploreState>();
    }

    public void LoadStats(Character _character)
    {
        playerName.text = _character.characterName;
        statusIndicator.SetHealth(_character.stats.curHealth, _character.stats.maxHealth);
        statusIndicator.SetAP(_character.stats.curAP, _character.stats.maxAP);
        statusIndicator.SetMP(_character.stats.curMP, _character.stats.maxMP);
    }

    public void ChangePlayer(Character character)
    {
        currentCharacter = character;
        if (onUnitChange != null)
            onUnitChange();

        LoadStats(currentCharacter);
    }

    public void CheckEndCondition()
    {
        if (battleEnemies.Count == 0)
        {
            ChangeState<VictorySequence>();
        }

        if (players.Count == 0)
        {
            ChangeState<DeathSequence>();
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