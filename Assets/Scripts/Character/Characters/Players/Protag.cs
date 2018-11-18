using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protag : Player
{

    private void Start()
    {
        stats.Init();
        characterName = "Protagonist";
        attackAbility = new FireballAbility(this);
        movementAbility = new WalkMovement(this, gc);
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }
}
