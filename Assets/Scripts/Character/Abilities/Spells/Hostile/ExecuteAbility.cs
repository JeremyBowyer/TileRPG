using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteAbility : TargetSpellAbility
{

    public ExecuteAbility(CharController _character)
    {
        AbilityName = "Execute";
        AbilityDescription = "Execute an enemy, if their HP is below 50%.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Physical, 500) };
        ApCost = 25;
        MpCost = 50;
        AbilityRange = 1f;
        diag = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = AbilityTypes.Intent.Hostile;
    }

    public override List<Node> GetRange()
    {
        List<Node> range = character.bc.pathfinder.FindRange(character.tile.node, AbilityRange, diag, true, false, false, false);
        return range;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Damage(new Damage[] { new Damage(DamageTypes.DamageType.Physical, character.character.stats.curHP) });
    }

    public override bool ValidateTarget(CharController character)
    {
        return (((float)character.character.stats.curHP / character.character.stats.maxHP) < 0.5);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        character.animParamController.SetTrigger("execute");
        character.animParamController.SetBool("idle");
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        callback();
        yield break;
    }
}