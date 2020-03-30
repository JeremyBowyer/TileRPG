using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : PartyMember
{
    public Wizard() : base()
    {
        avatar = null;

        abilities = new List<SpellAbility>();
        abilities.Add(new MagmaBallAbility(this));
        abilities.Add(new FireboltAbility(this));
        abilities.Add(new HealRayAbility(this));
        abilities.Add(new WallOfStoneAbility(this));

        attackAbility = new PunchAttackAbility(this);
        movementAbility = new TeleportMovement(this);
        movementAbility.costModifier = 3f;
    }

    public override void Init()
    {
        if (isInitialized) return;

        stats.maxHP = 100;
        stats.maxAP = 10000;
        stats.agility = 200;
        stats.Init();
        resists.Init();
        damageBonuses.Init();
        buildUps.Init();

        cName = "Wizard";
        cClass = "Wizard";
        isInitialized = true;
    }
}
