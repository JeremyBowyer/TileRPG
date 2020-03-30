using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedBladeAbility : TargetSpellAbility
{

    public CorruptedBladeAbility(Character _character)
    {
        AbilityName = "Corrupted Blade";
        AbilityDescription = "Melee attack that does corruption damage and applies rot.";
        AbilityDamage = new Damage[] { new Damage(_character, DamageTypes.DamageType.Corruption, 25, MaladyTypes.MaladyType.Rot, 100) };
        ApCost = 60;
        MpCost = 10;
        AbilityRange = 1f;
        diag = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/CorruptedBladeAbility");
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

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        if (controller is EnemyController)
            yield return new WaitForSeconds(2f);
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        controller.animParamController.SetTrigger("stab_attack", callback);
        controller.animParamController.SetBool("idle");
        yield break;
    }
}