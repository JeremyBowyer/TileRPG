using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealRayAbility : TargetSpellAbility
{

    public HealRayAbility(CharController _character)
    {
        AbilityName = "Heal Ray Ability";
        AbilityDescription = "Heal an ally for a moderate amount from range.";
        AbilityID = 2;
        AbilityPower = 50;
        ApCost = 25;
        MpCost = 40;
        AbilityRange = 60;
        diag = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = AbilityTypes.Intent.Support;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyEffect(CharController character)
    {
        character.Heal(AbilityPower);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        character.transform.LookAt(new Vector3(_target.tile.transform.position.x, character.transform.position.y, _target.tile.transform.position.z));
        character.animParamController.SetTrigger("cast_start");
        character.animParamController.SetBool("cast_loop");
        yield return new WaitForSeconds(1f);

        // Clean up
        character.transform.rotation = Quaternion.LookRotation(character.gc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        character.animParamController.SetBool("idle");
        character.animParamController.SetTrigger("cast_end");
        callback();
        yield break;
    }
}
