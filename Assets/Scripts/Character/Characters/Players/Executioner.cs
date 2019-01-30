using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Executioner : PartyMember
{
    public override void Init()
    {
        stats.maxHealth = 175;
        stats.maxAP = 100;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Executioner";
        cClass = "Executioner";

        isInitialized = true;
    }

    public override void InitAbilities()
    {
        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
        movementAbility.speed = 2f;
        movementAbility.costModifier = 3f;

        spells = new List<SpellAbility>();
        spells.Add(new ExecuteAbility(controller));
    }
}
