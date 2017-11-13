using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleController : StateMachine
{
    public CameraController cameraRig;
    public Grid grid;
    public Pathfinding pathfinder;
    public Node node;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Player currentCharacter;
    public Tile currentTile;
    public List<GameObject> players;
    public List<GameObject> enemies;
    public List<Character> characters = new List<Character>();

    public AbilityMenuPanelController abilityMenuPanelController;
    public Turn turn = new Turn();

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        ChangeState<InitBattleState>();
    }
}