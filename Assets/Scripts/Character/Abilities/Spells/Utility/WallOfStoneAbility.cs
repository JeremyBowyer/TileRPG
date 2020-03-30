using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public WallOfStoneAbility(Character _character)
    {
        AbilityName = "Wall of Stone";
        AbilityDescription = "Raise a wall of stone, which prevents ground movement.";
        AbilityDamage = null;
        ApCost = 40;
        MpCost = 10;
        AbilityRange = 7;
        AbilityPathLimit = 4;
        diag = true;
        ignoreOccupant = false;
        ignoreUnwalkable = false;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("GridClick");
        abilityIntent = IntentTypes.Intent.Support;
    }

    public override List<Node> GetRange()
    {
        List<Node> range = controller.bc.pathfinder.FindRange(controller.tile.node, AbilityRange, diag, ignoreOccupant, true, false, true);
        return range;
    }

    public override List<Node> GetPath(Tile _start, Tile _end)
    {
        List<Node> path = controller.bc.pathfinder.FindLine(_start.node, _end.node, AbilityPathLimit, ignoreOccupant, ignoreUnwalkable);
        return path;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
    }

    public override void ApplyTileEffect(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        if (_tile.Occupant != null)
        {
            CharController character = _tile.Occupant.GetComponent<CharController>();
            if (character != null)
                ApplyCharacterEffect(character);
        }

        TileEffect oldEffect = _tile.GetComponent<TileEffect>();
        if (oldEffect != null)
            oldEffect.RemoveEffect();
        TileEffect newEffect = _tile.gameObject.AddComponent<WallOfStoneTileEffect>();
        newEffect.Init(_tile, _sourceDirection, _grid, character);
    }

    public override IEnumerator Initiate(Tile tile, List<Node> affectedArea, Action callback)
    {
        controller.animParamController.SetTrigger("cast_start");
        controller.animParamController.SetBool("cast_loop");
        controller.transform.LookAt(new Vector3(tile.transform.position.x, controller.transform.position.y, tile.transform.position.z));
        
        //inProgress = true;
        yield return new WaitForSeconds(1);

        controller.animParamController.SetBool("idle");
        controller.animParamController.SetTrigger("cast_end");
        callback();
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, tile.node), Vector3.up);
        yield break;
    }

}