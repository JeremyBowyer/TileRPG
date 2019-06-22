﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : PartyMember
{
    public override void Init()
    {
        if (isInitialized) return;

        stats.maxHealth = 100;
        stats.maxAP = 10000;
        stats.agility = 200;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Wizard";
        cClass = "Wizard";
        isInitialized = true;
    }

    public override void InitAbilities()
    {
        spells = new List<SpellAbility>();
        spells.Add(new MagmaBallAbility(controller));
        spells.Add(new FireboltAbility(controller));
        spells.Add(new HealRayAbility(controller));
        spells.Add(new WallOfStoneAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new TeleportMovement(controller);
        movementAbility.costModifier = 3f;
    }
}
