using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAbility : AttackAbility
{

    public MeleeAbility(Character _character)
    {
        AbilityName = "Melee attack";
        AbilityDescription = "Attack at melee range.";
        AbilityID = 2;
        AbilityPower = 25;
        AbilityCost = 25;
        AbilityRange = 14;
        diag = true;
        character = _character;
    }

    public override IEnumerator Initiate(Character _target, Action callback)
    {
        character.transform.rotation = Quaternion.LookRotation(character.gc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        inProgress = true;
        character.animParamController.SetTrigger("attack");
        _target.Damage(AbilityPower);
        character.animParamController.SetBool("idle");
        inProgress = false;
        callback();
        yield break;
    }
}