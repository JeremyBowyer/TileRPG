using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : Enemy
{
    public override void Init()
    {
        stats.maxHP = 100;
        stats.maxAP = 100;
        stats.agility = 40;
        stats.Init();
        resists.Init();
        buildUps.Init();

        experience = 0;
        level = 1;

        cName = "Druid";
        cClass = "Druid";

        spells = new List<SpellAbility>();
        spells.Add(new HealRayAbility(controller));

        attackAbility = new ArrowAbility(controller);
        movementAbility = new WalkMovement(controller);
    }
}
