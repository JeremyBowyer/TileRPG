using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : PartyMember
{
    public override void Init()
    {
        if (isInitialized) return;

        stats.maxHealth = 100;
        stats.maxAP = 100;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Rogue";
        cClass = "Rogue";
        isInitialized = true;
    }

    public override void InitAbilities()
    {
        attackAbility = new MeleeAbility(controller);
        movementAbility = new TeleportMovement(controller);
        movementAbility.costModifier = 2f;

        spells = new List<SpellAbility>();
        spells.Add(new BackstabAbility(controller));
    }
}
