using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedBladeAbility : TargetSpellAbility
{

    public CorruptedBladeAbility(CharController _character)
    {
        AbilityName = "Corrupted Blade";
        AbilityDescription = "Melee attack that does corruption damage and applies rot.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Corruption, 25, MaladyTypes.MaladyType.Rot, 100) };
        ApCost = 60;
        MpCost = 10;
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
        character.Damage(AbilityDamage);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        if (character is EnemyController)
            yield return new WaitForSeconds(2f);
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        character.animParamController.SetTrigger("stab", callback);
        character.animParamController.SetBool("idle");
        yield break;
    }
}