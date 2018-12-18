using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : PartyMember
{
    public override void Init()
    {
        stats.maxHealth = 100;
        stats.maxAP = 100;
        stats.Init();
        stats.curHealth = 25;
        experience = 0;
        level = 1;

        cClass = "Rogue";

        attackAbility = new MeleeAbility(controller);
        movementAbility = new TeleportMovement(controller);
    }
}
