using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMaster : MonoBehaviour {

	// References
	private Pathfinding pathfinder;
    private Text playerName;
    private Grid grid;

    public List<GameObject> players;
    public List<GameObject> enemies;
    private List<GameObject> startingTiles;
    public StatusIndicator statusIndicator;
    public Player curPlayer;
	public bool paused;
    public BattleState currentState;
    public enum BattleState
    {
        START,
        PLAYERCHOICE,
        PLAYERMOVE,
        PLAYERATTACK,
        ENEMYCHOICE,
        ENEMYMOVE,
        ENEMYATTACK,
        LOSE,
        WIN
    }


    void Awake () {

        // Instantiate Variables
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        startingTiles = new List<GameObject> (GameObject.FindGameObjectsWithTag ("StartingTile"));
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        playerName = GameObject.Find("PlayerName").GetComponent<Text>();

        // Check for status indicator
        if (statusIndicator == null)
        {
            Debug.LogError("Status indicator not assigned to GameMaster.");
        }

		// Place players on starting tiles
		if (players.Count > startingTiles.Count) {
			Debug.LogError ("Not enough starting tiles for players");
		} else {
			for (int i = 0; i < players.Count; i++) {
				GameObject player = players [i];
				GameObject tile = startingTiles [i];
				float height = tile.GetComponent<BoxCollider> ().bounds.extents.z * 2;
                player.transform.position = tile.transform.position + new Vector3 (0, height, 0);
                tile.GetComponent<Tile>().occupant = player;
			}
		}

        currentState = BattleState.START;
	}
	
	// Update is called once per frame
	void Update () {
        
        switch (currentState)
        {
            case (BattleState.START):
                NextPlayer();
                break;
            case (BattleState.PLAYERCHOICE):
                break;
            case (BattleState.PLAYERMOVE):
                break;
            case (BattleState.PLAYERATTACK):
                break;
            case (BattleState.ENEMYCHOICE):
                break;
            case (BattleState.ENEMYMOVE):
                break;
            case (BattleState.ENEMYATTACK):
                break;
            case (BattleState.LOSE):
                break;
            case (BattleState.WIN):
                break;
            default:
                break;
        }

    }

	public void NextPlayer() {

        currentState = BattleState.PLAYERCHOICE;
        grid.ResetRange();

		if (curPlayer == null) {
			curPlayer = players [0].GetComponent<Player>();
		} else {
			int curPlayerIndex = players.IndexOf (curPlayer.gameObject);
			if (curPlayerIndex == players.Count - 1) {
				curPlayer = players [0].GetComponent<Player>();
			} else {
				curPlayer = players [curPlayerIndex + 1].GetComponent<Player>();
			}
		}
        curPlayer.fillAP();
        LoadStats(curPlayer);
    }

    void LoadStats(Player _player)
    {
        playerName.text = _player.playerName;
        statusIndicator.SetHealth(_player.stats.curHealth, _player.stats.maxHealth);
        statusIndicator.SetAP(_player.stats.curAP, _player.stats.maxAP);
        statusIndicator.SetMP(_player.stats.curMP, _player.stats.maxMP);
    }

    public void CheckMoveRange(Player _player, bool diag)
    {
        Transform _transform = _player.gameObject.transform;
        int _limit = _player.stats.moveRange;
        grid.path = null;
        grid.range = null;
        pathfinder.FindRange(_transform.position, _limit, diag);
    }

    public void CheckAttackRange(Player _player, BaseAbility _ability, bool diag)
    {
        Transform _transform = _player.gameObject.transform;
        int _limit = _ability.AbilityRange;
        grid.path = null;
        grid.range = null;
        pathfinder.FindRange(_transform.position, _limit, diag);
    }

    public void PlayerMove()
    {
        currentState = BattleState.PLAYERMOVE;
        CheckMoveRange(curPlayer, false);
    }

    public void PlayerAttack()
    {
        currentState = BattleState.PLAYERATTACK;
        CheckAttackRange(curPlayer, curPlayer.curAbility, false);
    }

}
