using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protagonist : PartyMember
{
    public List<PartyMember> partyMembers = new List<PartyMember>();
    public Inventory inventory;

    public override void Init()
    {
        if (isInitialized) return;

        stats.maxHealth = 1000;
        stats.maxAP = 10000;
        stats.agility = 75;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Protagonist";
        cClass = "Handsome";

        inventory = new Inventory();
        inventory.Add(new Potion());
        inventory.Add(new Potion());
        inventory.Add(new Potion());

        isInitialized = true;
    }

    public override void InitAbilities()
    {
        spells = new List<SpellAbility>();
        spells.Add(new MagmaBallAbility(controller));
        spells.Add(new FireboltAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
    }
}
