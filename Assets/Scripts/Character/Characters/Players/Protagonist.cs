using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protagonist : PartyMember
{
    public List<PartyMember> partyMembers = new List<PartyMember>();
    public Inventory inventory;

    public override void Init()
    {
        stats.maxHealth = 1000;
        stats.maxAP = 10000;
        stats.Init();

        experience = 0;
        level = 1;

        cName = "Protagonist";
        cClass = "Handsome";

        spells = new List<SpellAbility>();
        spells.Add(new MagmaBallAbility(controller));
        spells.Add(new FireboltAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);

        Rogue member1 = new Rogue()
        {
            cName = "Wingus",
            cClass = "Rogue",
            model = "Character_Male_Rouge_01"
        };
        partyMembers.Add(member1);

        Rogue member2 = new Rogue()
        {
            cName = "Dingus",
            cClass = "Rogue",
            model = "Character_Female_Gypsy"
        };
        partyMembers.Add(member2);

        Wizard member3 = new Wizard()
        {
            cName = "Son of Kong",
            cClass = "Wizard",
            model = "Character_Male_Wizard"
        };
        partyMembers.Add(member3);

        inventory = new Inventory();
        inventory.Add(new Potion());
        inventory.Add(new Potion());
        inventory.Add(new Potion());
    }
}
