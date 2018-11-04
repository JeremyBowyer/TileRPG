using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : BaseAbility
{

    public AttackAbility(Character _character)
    {
        AbilityName = "Phyiscal Attack";
        AbilityDescription = "A character's base physical attack.";
        AbilityID = 1;
        AbilityPower = 50;
        AbilityCost = 10;
        AbilityRange = 10;
        character = _character;
        diag = false;
    }

    public override IEnumerator Initiate(Character target)
    {
        yield break;
    }

}
