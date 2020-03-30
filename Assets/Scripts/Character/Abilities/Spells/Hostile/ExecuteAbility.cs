using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteAbility : TargetSpellAbility
{

    public ExecuteAbility(Character _character)
    {
        AbilityName = "Execute";
        AbilityDescription = "Execute an enemy, if their HP is below 50%.";
        AbilityDamage = new Damage[] { new Damage(_character, DamageTypes.DamageType.Physical, int.MaxValue) };
        ApCost = 25;
        MpCost = 50;
        AbilityRange = 1f;
        diag = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/ExecuteAbility");
    }

    public override List<Node> GetRange()
    {
        List<Node> range = controller.bc.pathfinder.FindRange(controller.tile.node, AbilityRange, diag, true, false, false, false);
        return range;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController _target)
    {
        foreach (Damage dmg in AbilityDamage)
        {
            dmg.ability = this;
            dmg.damageAmount = _target.Stats.curHP;
        }
        character.controller.DealDamage(AbilityDamage, _target);
    }

    public override bool ValidateTarget(CharController character)
    {
        return (((float)character.character.stats.curHP / character.character.stats.maxHP) < 0.5);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        controller.animParamController.SetTrigger("overhead_attack", callback);
        controller.animParamController.SetBool("idle");
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        yield break;
    }
}