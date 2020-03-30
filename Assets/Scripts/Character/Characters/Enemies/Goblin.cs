using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    public override void Init()
    {
        stats.maxHP = 200;
        stats.maxAP = 125;
        stats.agility = 25;
        stats.Init();
        resists.Init();
        damageBonuses.Init();
        buildUps.Init();

        isInitialized = true;

        cName = "Rob";
        cClass = "Goblin";

        abilities = new List<SpellAbility>();

        attackAbility = new PunchAttackAbility(this);
        movementAbility = new WalkMovement(this);
    }
}
