﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackstabAbility : TargetSpellAbility
{

    public BackstabAbility(CharController _character)
    {
        AbilityName = "Backstab";
        AbilityDescription = "Attack an enemy from behind, dealing high damage";
        AbilityPower = 25;
        ApCost = 25;
        MpCost = 40;
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
        _target.Damage(AbilityPower);
    }

    public override bool ValidateTarget(CharController _character)
    {
        return character.bc.grid.CompareDirection(character.tile.node, _character.tile.node, _character.direction) == Grid.Position.Back;
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        character.animParamController.SetTrigger("backstab");
        character.animParamController.SetBool("idle");
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        callback();
        yield break;
    }
}
