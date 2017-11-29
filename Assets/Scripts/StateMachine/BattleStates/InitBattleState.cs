using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InitBattleState : BattleState
{
    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();


    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        grid.CreateGrid();

        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        startingTilesPlayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("StartingTilePlayer"));
        startingTilesEnemy = new List<GameObject>(GameObject.FindGameObjectsWithTag("StartingTileEnemy"));

        if(players.Count > startingTilesPlayer.Count)
        {
            Debug.LogError("Not enough starting tiles for players.");
        }

        if (enemies.Count > startingTilesEnemy.Count)
        {
            Debug.LogError("Not enough starting tiles for enemies.");
        }

        // Place players on starting tiles
        for (int i = 0; i < startingTilesPlayer.Count; i++)
        {
            GameObject player = players[i];
            characters.Add(player.GetComponent<Player>());
            Tile tile = startingTilesPlayer[i].GetComponent<Tile>();
            player.GetComponent<Player>().Place(tile, 0);
            tile.GetComponent<Tile>().occupant = player;
        }

        // Place players on starting tiles
        for (int i = 0; i < startingTilesEnemy.Count; i++)
        {
            GameObject enemy = enemies[i];
            characters.Add(enemy.GetComponent<Enemy>());
            Tile tile = startingTilesEnemy[i].GetComponent<Tile>();
            enemy.GetComponent<Enemy>().Place(tile, 0);
            tile.GetComponent<Tile>().occupant = enemy;
        }
        owner.players = players;
        owner.enemies = enemies;
        owner.characters = characters;
        owner.currentCharacter = players[0].GetComponent<Character>();
        yield return null;
        owner.ChangeState<SelectUnitState>();
    }
}