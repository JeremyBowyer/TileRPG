using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharController {

    public override void Die(Damage _damage)
    {
        bc.players.Remove(gameObject);
        lc.players.Remove(gameObject);
        bc.rc.roundChars.Remove(this);
        base.Die(_damage);
    }

    public override void CreateCharacter()
    {
        // Leave blank so base CreateCharacter won't run
    }

}
