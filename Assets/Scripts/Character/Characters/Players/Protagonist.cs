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

        stats.maxHP = 1000;
        stats.maxAP = 10000;
        stats.agility = 75;
        stats.Init();
        resists.Init();
        damageBonuses.Init();
        buildUps.Init();

        cName = "Protagonist";
        cClass = "Handsome";

        inventory = new Inventory();
        inventory.Add(new Potion());
        inventory.Add(new Potion());
        inventory.Add(new Potion());

        isInitialized = true;
    }
}
