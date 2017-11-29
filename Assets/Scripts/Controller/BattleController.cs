﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleController : StateMachine
{
    public CameraController cameraRig;
    public UIController uiController;
    public Grid grid;
    public Pathfinding pathfinder;
    public Node node;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Character currentCharacter;
    public Tile currentTile;
    public List<GameObject> players;
    public List<GameObject> enemies;
    public List<Character> characters = new List<Character>();
    public Text playerName;
    public StatusIndicator statusIndicator;
    public AbilityMenuPanelController abilityMenuPanelController;
    public Turn turn = new Turn();

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();

        if (uiController == null)
            Debug.LogError("UIController not assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("StatusIndicator not assigned to " + gameObject.name);

        ChangeState<InitBattleState>();
    }

    public void LoadStats(Character _character)
    {
        Player _player = _character as Player;
        playerName.text = _player.playerName;
        statusIndicator.SetHealth(_player.stats.curHealth, _player.stats.maxHealth);
        statusIndicator.SetAP(_player.stats.curAP, _player.stats.maxAP);
        statusIndicator.SetMP(_player.stats.curMP, _player.stats.maxMP);
    }

    public void ChangePlayer(Character character)
    {
        currentCharacter = character;
        currentCharacter.fillAP();
        Debug.Log(currentCharacter.gameObject.name);
    }

}