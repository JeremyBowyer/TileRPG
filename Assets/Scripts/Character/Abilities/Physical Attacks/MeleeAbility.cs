using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAbility : AttackAbility
{
    public MeleeAbility(CharController _character)
    {
        AbilityName = "Melee attack";
        AbilityDescription = "Attack at melee range.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Physical, 30) };
        ApCost = 75;
        AbilityRange = 1.5f;
        diag = true;
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
        return _owner.Stats.curAP >= ApCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Damage(AbilityDamage);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        if(character is EnemyController)
            yield return new WaitForSeconds(2f);
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        character.animParamController.SetTrigger("attack", callback);
        character.animParamController.SetBool("idle");
        yield break;
    }
}