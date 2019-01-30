using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : CharController
{
    public override void Die()
    {
        bc.enemies.Remove(this.gameObject);
        base.Die();
    }
}
