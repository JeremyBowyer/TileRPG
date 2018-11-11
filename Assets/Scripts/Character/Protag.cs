using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protag : Player
{

    private void Start()
    {
        stats.Init();
        playerName = "Protagonist";
        attackAbility = new FireballAbility(this);
        movementAbility = new WalkMovement(this, gc);
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }
}
