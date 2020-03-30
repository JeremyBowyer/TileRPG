using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealRayAbility : TargetSpellAbility
{

    public HealRayAbility(Character _character)
    {
        AbilityName = "Heal Ray Ability";
        AbilityDescription = "Heal an ally for a moderate amount from range.";
        AbilityDamage = null;
        ApCost = 25;
        MpCost = 40;
        AbilityRange = 60;
        diag = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Heal;
    }

    public override List<Node> GetRange()
    {
        List<Node> range = controller.bc.pathfinder.FindGeometricRange(controller.tile.node, AbilityRange);
        return range;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Heal(50, true);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        controller.transform.LookAt(new Vector3(_target.tile.transform.position.x, controller.transform.position.y, _target.tile.transform.position.z));
        controller.animParamController.SetTrigger("cast_start");
        controller.animParamController.SetBool("cast_loop");
        yield return new WaitForSeconds(1f);

        // Clean up
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        controller.animParamController.SetBool("idle");
        controller.animParamController.SetTrigger("cast_end");
        callback();
        yield break;
    }
}
