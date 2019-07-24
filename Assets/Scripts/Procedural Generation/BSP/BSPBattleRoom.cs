using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPBattleRoom : BSPRoom
{
    public bool completed;
    private string[] enemyPrefabs;
    private GameObject[] enemies;
    private int enemyCnt;

    public override void Init(int _id, Vector2 _bufferBounds)
    {
        base.Init(_id, _bufferBounds);
        //enemyPrefabs = new string[] { "Druid", "Goblin_Shaman", "Goblin_Warchief" };
        enemyPrefabs = new string[] { "Goblin_Warchief", "Goblin_Warchief", "Goblin_Warchief" };
        completed = false;

        float area = xSize * zSize / 60;
        enemyCnt = Mathf.RoundToInt(area);

        enemies = new GameObject[enemyCnt];
    }

    public void AddEnemies()
    {
        List<GameObject> eligibleFloors = new List<GameObject>();

        foreach(GameObject floor in floors)
        {
            BSPFloor floorObj = floor.GetComponent<BSPFloor>();
            if (floorObj.prop == null && !floorObj.isDoorway)
                eligibleFloors.Add(floor);
        }

        for (int i = 0; i<Math.Min(enemyCnt, floors.Count); i++)
        {
            int enemyIdx = rnd.Next(0, 3);
            int floorIdx = rnd.Next(0, eligibleFloors.Count);

            GameObject enemyGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Characters/Enemies/" + enemyPrefabs[enemyIdx]));
            enemyGO.transform.position = eligibleFloors[floorIdx].transform.Find("AnchorPoint").transform.position;
            eligibleFloors.RemoveAt(floorIdx);
            enemyGO.transform.localScale = enemyGO.transform.localScale * 0.5f;
            EnemyController controller = enemyGO.GetComponent<EnemyController>();
            controller.room = this;

            enemies[i] = enemyGO;
            AddCharacter(enemyGO);
        }
    }

    private void OnDrawGizmos()
    {
        if (lc.bc.grid.grid != null)
        {
            Gizmos.color = CustomColors.Hostile;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, transform.position + lc.bc.grid.forwardDirection * 8f + Vector3.up * 2f);

            Gizmos.color = CustomColors.Support;
            Gizmos.DrawLine(transform.position + Vector3.up * 2f, transform.position + lc.bc.grid.rightDirection * 8f + Vector3.up * 2f);
        }

        if(gameObject.activeSelf && !completed)
        {
            Gizmos.color = CustomColors.Fire;
            Gizmos.DrawCube(transform.position, new Vector3(xSize, ySize, zSize));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Protag" && !completed)
        {
            lc.StartBattle(this);
        }
    }
}
