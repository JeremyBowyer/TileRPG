﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitPlacementState : BattleState
{
    List<Node> moveRange;
    CharController character;
    Movement mover;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(PlaceUnitsState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        character = args.character;
        mover = character.MovementAbility;
        moveRange = mover.GetNodesInRange(character.Stats.moveRange, true, false, mover.costModifier);
        grid.HighlightNodes(moveRange);
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        grid.UnHighlightNodes(moveRange);
        //grid.SelectNodes(moveRange, Color.grey);
        grid.DeSelectNodes();
        moveRange = null;
        battleUiController.SetApCost();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("GridClick");
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Tile tile = e.info.collider.gameObject.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.Log("Select a tile.");
            return;
        }

        if (moveRange.Contains(tile.node))
        {
            /*
            character.Place(tile);
            character.gameObject.transform.rotation = Quaternion.LookRotation(gc.grid.forwardDirection, Vector3.up);
            */

            List<Node> path = gc.pathfinder.FindPath(
                character.tile.node,
                tile.node,
                character.Stats.moveRange,
                character.MovementAbility.diag,
                character.MovementAbility.ignoreOccupant,
                character.MovementAbility.ignoreUnwalkable,
                false,
                character.MovementAbility.costModifier);

            StateArgs moveArgs = new StateArgs
            {
                path = path,
                waitingStateMachines = new List<StateMachine> { gc }
            };
            character.ChangeState<MoveSequenceState>(moveArgs);
            gc.ChangeState<PlaceUnitsState>();
            return;
        }
        else
        {
            Debug.Log("Select a valid tile.");
        }
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (moveRange.Contains(tile.node))
        {
            List<Node> path = pathfinder.FindPath(character.tile.node, tile.node, character.Stats.moveRange, mover.diag, false, mover.ignoreUnwalkable, false, mover.costModifier);
            if (mover.isPath)
            {
                grid.SelectNodes(path, Color.cyan);
            }
            else
            {
                grid.SelectNodes(path[path.Count - 1], Color.cyan);
            }

            battleUiController.SetApCost(path[path.Count - 1].gCost, character.Stats.moveRange);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.transform.GetComponent<Tile>();

        if (tile == null)
            return;

        if (moveRange.Contains(tile.node))
        {
            grid.DeSelectNodes();
            battleUiController.SetApCost();
        }
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

}