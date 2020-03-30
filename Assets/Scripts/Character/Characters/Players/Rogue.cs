using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : PartyMember
{
    public Rogue() : base()
    {
        avatar = null;
        attackAbility = new PunchAttackAbility(this);
        movementAbility = new TeleportMovement(this);
        movementAbility.costModifier = 2f;

        abilities = new List<SpellAbility>();
        abilities.Add(new BackstabAbility(this));
    }

    public override void Init()
    {
        if (isInitialized) return;

        stats.maxHP = 100;
        stats.maxAP = 100;
        stats.Init();
        resists.Init();
        damageBonuses.Init();
        buildUps.Init();

        cName = "Rogue";
        cClass = "Rogue";
        isInitialized = true;
    }

}
