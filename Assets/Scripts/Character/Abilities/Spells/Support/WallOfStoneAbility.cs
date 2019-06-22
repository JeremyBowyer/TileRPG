using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfStoneAbility : EnvironmentPathSpellAbility
{

    private int abilityPathLimit;
    public override int AbilityPathLimit
    {
        get
        {
            return abilityPathLimit;
        }
        set
        {
            abilityPathLimit = value;
        }
    }

    public WallOfStoneAbility(CharController _character)
    {
        AbilityName = "Wall of Stone";
        AbilityDescription = "Raise a wall of stone, which prevents ground movement.";
        AbilityPower = 0;
        ApCost = 40;
        MpCost = 10;
        AbilityRange = 7;
        AbilityPathLimit = 4;
        diag = true;
        ignoreOccupant = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("GridClick");
        abilityIntent = AbilityTypes.Intent.Hostile;
    }

    public override List<Node> GetRange()
    {
        List<Node> range = character.bc.pathfinder.FindRange(character.tile.node, AbilityRange, diag, ignoreOccupant, true, false, true);
        return range;
    }

    public override List<Node> GetPath(Tile _start, Tile _end)
    {
        List<Node> path = character.bc.pathfinder.FindLine(_start.node, _end.node, AbilityPathLimit, ignoreOccupant, true);
        return path;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
    }

    public override void ApplyTileEffect(Tile tile)
    {
        if (tile.Occupant != null)
        {
            CharController character = tile.Occupant.GetComponent<CharController>();
            if (character != null)
                ApplyCharacterEffect(character);
        }

        TileEffect oldEffect = tile.GetComponent<TileEffect>();
        if (oldEffect != null)
            oldEffect.RemoveEffect();
        TileEffect newEffect = tile.gameObject.AddComponent<WallOfStoneTileEffect>();
        newEffect.Init(tile);
    }

    public override IEnumerator Initiate(Tile tile, Action callback)
    {
        character.animParamController.SetTrigger("cast_start");
        character.animParamController.SetBool("cast_loop");
        character.transform.LookAt(new Vector3(tile.transform.position.x, character.transform.position.y, tile.transform.position.z));
        
        inProgress = true;
        yield return new WaitForSeconds(1);

        character.animParamController.SetBool("idle");
        character.animParamController.SetTrigger("cast_end");
        callback();
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, tile.node), Vector3.up);
        yield break;
    }
}