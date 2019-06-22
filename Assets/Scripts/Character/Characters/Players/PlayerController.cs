using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharController {

    public override void Awake ()
    {
        base.Awake();
    }

    public override void Die()
    {
        bc.players.Remove(this.gameObject);
        bc.characters.Remove(this.gameObject);
        bc.rc.roundChars.Remove(this);
        base.Die();
    }

    public override void CreateCharacter()
    {
        // Leave blank so base CreateCharacter won't run
    }

}
