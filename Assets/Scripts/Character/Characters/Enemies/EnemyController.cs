using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : CharController
{
    public override void Die()
    {
        gc.worldEnemies.Remove(this.gameObject);
        gc.battleEnemies.Remove(this.gameObject);
        base.Die();
    }
}
