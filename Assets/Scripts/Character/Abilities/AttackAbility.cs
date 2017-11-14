using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : BaseAbility
{

    public AttackAbility()
    {
        AbilityName = "Phyiscal Attack";
        AbilityDescription = "A character's base physical attack.";
        AbilityID = 1;
        AbilityPower = 50;
        AbilityCost = 10;
        AbilityRange = 10;
    }
}
