using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackstabAbility : TargetSpellAbility
{

    public BackstabAbility(Character _character)
    {
        AbilityName = "Backstab";
        AbilityDescription = "Attack an enemy from behind, dealing high damage";
        AbilityDamage = new Damage[] { new Damage(_character, DamageTypes.DamageType.Physical, 40) };
        ApCost = 25;
        MpCost = 40;
        AbilityRange = 1f;
        diag = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/BackstabAbility");
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
        }
        character.controller.DealDamage(AbilityDamage, _target);
    }

    public override bool ValidateTarget(CharController _target)
    {
        Grid.Position pos = controller.bc.grid.CompareDirection(controller.tile.node, _target.tile.node, _target.direction);

        return pos == Grid.Position.Back;
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        controller.animParamController.SetTrigger("stab_attack");
        controller.animParamController.SetBool("idle");
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        callback();
        yield break;
    }
}
