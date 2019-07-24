using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    public override void Init()
    {
        stats.maxHealth = 200;
        stats.maxAP = 125;
        stats.agility = 25;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Rob";
        cClass = "Goblin";

        spells = new List<SpellAbility>();

        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
    }
}
