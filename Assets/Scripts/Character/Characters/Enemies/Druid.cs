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
        damageBonuses.Init();
        buildUps.Init();

        cName = "Druid";
        cClass = "Druid";

        abilities = new List<SpellAbility>();
        abilities.Add(new HealRayAbility(this));

        attackAbility = new ArrowAbility(this);
        movementAbility = new WalkMovement(this);
    }
}
