using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    void Start()
    {
        base.Awake();
        stats.Init();
        attackAbility = new MeleeAbility(this);
        movementAbility = new TeleportMovement(this, gc);
    }

    public override void Die()
    {
        gc.worldEnemies.Remove(this.gameObject);
        gc.battleEnemies.Remove(this.gameObject);
        gc.characters.Remove(this.gameObject);
        Destroy(this.gameObject);
        base.Die();
    }
}
