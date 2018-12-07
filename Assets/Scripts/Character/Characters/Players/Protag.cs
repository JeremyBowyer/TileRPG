using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protag : Player
{

    private void Start()
    {
        base.Awake();
        stats.maxHealth = 1000;
        stats.maxAP = 10000;
        stats.Init();
        characterName = "Protagonist";
        spells.Add(new MagmaBallAbility(this));
        spells.Add(new FireboltAbility(this));
        attackAbility = new ArrowAbility(this);
        movementAbility = new WalkMovement(this, gc);
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }
}
