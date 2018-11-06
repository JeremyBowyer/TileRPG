using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    public string playerName;

    void Start () {
        stats.Init();
        attackAbility = new ArrowAbility(this);
        movementAbility = new WalkMovement(this, gc);
    }

    public override void Die()
    {
        gc.players.Remove(this.gameObject);
        gc.characters.Remove(this.gameObject);
        Destroy(this.gameObject);

        base.Die();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.rotation * Vector3.right);
    }
}
