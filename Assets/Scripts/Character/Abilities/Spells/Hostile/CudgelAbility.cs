using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CudgelAbility : TargetSpellAbility
{

    public CudgelAbility(Character _character)
    {
        AbilityName = "Cudgel";
        AbilityDescription = "Beat a man upon his head.";
        AbilityDamage = new Damage[] { new Damage(_character, DamageTypes.DamageType.Bludgeon, 20, MaladyTypes.MaladyType.Concussion, 40) };
        ApCost = 50;
        MpCost = 10;
        AbilityRange = 1f;
        diag = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/CudgelAbility");
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
        controller.animParamController.SetTrigger("blunt_attack", callback);
        controller.animParamController.SetBool("idle");
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        yield break;
    }
}
