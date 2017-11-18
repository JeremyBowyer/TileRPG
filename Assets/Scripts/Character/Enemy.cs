using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public string enemyName;
    public BaseAbility curAbility;

    // References
    private BattleController bc;
    private StatusIndicator statusIndicator;

    // Use this for initialization
    void Start()
    {
        stats.Init();
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        statusIndicator = transform.Find("CameraAngleTarget").Find("StatusIndicator").GetComponent<StatusIndicator>();
        curAbility = new AttackAbility();
    }

    public override void Damage(int amt)
    {
        stats.curHealth -= amt;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        if (stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        bc.enemies.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
