using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public string enemyName;
    public BaseAbility curAbility;
    
    // Use this for initialization
    void Start()
    {
        stats.Init();
        curAbility = new AttackAbility();
    }

    public override void Die()
    {
        bc.enemies.Remove(this.gameObject);
        bc.characters.Remove(this);
        Destroy(this.gameObject);
    }
}
