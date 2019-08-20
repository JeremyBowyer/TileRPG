using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CudgelAbility : TargetSpellAbility
{

    public CudgelAbility(CharController _character)
    {
        AbilityName = "Cudgel";
        AbilityDescription = "Beat a man upon his head.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Bludgeon, 20, MaladyTypes.MaladyType.Concussion, 40) };
        ApCost = 50;
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

    public override void ApplyCharacterEffect(CharController _target)
    {
        _target.Damage(AbilityDamage);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        if (character is EnemyController)
            yield return new WaitForSeconds(2f);
        character.animParamController.SetTrigger("attack_overhead", callback);
        character.animParamController.SetBool("idle");
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        yield break;
    }
}
