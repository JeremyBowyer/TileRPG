using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : Enemy
{
    public override void Init()
    {
        stats.maxHealth = 100;
        stats.maxAP = 100;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Drew";
        cClass = "Druid";

        spells = new List<SpellAbility>();
        spells.Add(new FireboltAbility(controller));

        attackAbility = new ArrowAbility(controller);
        movementAbility = new WalkMovement(controller);
    }
}
