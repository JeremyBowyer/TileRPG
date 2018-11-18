using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : StateMachine
{
    public CameraController cameraRig;
    public Camera _camera;
    public CameraRotate _cameraController;
    public BattleUIController uiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public Node node;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Character currentCharacter;
    public Player protag;
    public Tile currentTile;
    public List<GameObject> players;
    public List<GameObject> enemies;
    public List<GameObject> worldEnemies;
    public List<GameObject> characters;
    public Text playerName;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public Enemy battleInitiator;
    public Vector3 protagStartPos = new Vector3(0, 0, 0);

    void Start()
    {
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<Player>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        _cameraController = GameObject.Find("Camera").GetComponent<CameraRotate>();
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

        if (uiController == null)
            Debug.LogError("UIController not assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("StatusIndicator not assigned to " + gameObject.name);

        ChangeState<WorldExploreState>();
    }

    public void LoadStats(Character _character)
    {
        Player _player = _character as Player;
        playerName.text = _player.characterName;
        statusIndicator.SetHealth(_player.stats.curHealth, _player.stats.maxHealth);
        statusIndicator.SetAP(_player.stats.curAP, _player.stats.maxAP);
        statusIndicator.SetMP(_player.stats.curMP, _player.stats.maxMP);
    }

    public void ChangePlayer(Character character)
    {
        currentCharacter = character;
        currentCharacter.fillAP();
    }

    public void CheckEndCondition()
    {
        if (enemies.Count == 0)
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