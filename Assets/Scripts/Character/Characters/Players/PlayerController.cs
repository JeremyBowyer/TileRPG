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
        gc.players.Remove(this.gameObject);
        gc.characters.Remove(this.gameObject);
        base.Die();
    }

    public override void CreateCharacter()
    {
        // Leave blank so base CreateCharacter won't run
    }

}
