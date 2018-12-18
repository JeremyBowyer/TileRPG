using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : PartyMember
{
    public override void Init()
    {
        stats.maxHealth = 100;
        stats.maxAP = 100;
        stats.Init();

        experience = 0;
        level = 1;

        cClass = "Wizard";

        spells = new List<SpellAbility>();
        spells.Add(new MagmaBallAbility(controller));
        spells.Add(new FireboltAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new TeleportMovement(controller);
    }
}
