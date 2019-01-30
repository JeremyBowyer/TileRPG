using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveTargetState : BattleState
{
    List<Node> moveRange;
    PlayerController player;
    Movement mover;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(MoveSequenceState),
            typeof(CommandSelectionState),
            typeof(CheckForTurnEndState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        mover = bc.CurrentCharacter.MovementAbility;
        player = bc.CurrentCharacter as PlayerController;
        moveRange = mover.GetNodesInRange(player.Stats.moveRange, mover.diag, false, mover.costModifier);
        grid.SelectNodes(moveRange, CustomColors.MovementRange, "moverange");
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        bc.lineRenderer.positionCount = 0;
        grid.DeSelectNodes("movepath");
        grid.DeSelectNodes("moverange");
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
            List<Node> path = bc.pathfinder.FindPath(
                bc.CurrentCharacter.tile.node,
                tile.node,
                player.Stats.moveRange,
                player.MovementAbility.diag,
                player.MovementAbility.ignoreOccupant,
                player.MovementAbility.ignoreUnwalkable,
                false,
                player.MovementAbility.costModifier);

            StateArgs moveArgs = new StateArgs
            {
                path = path,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            bc.CurrentCharacter.ChangeState<MoveSequenceState>(moveArgs);
            bc.ChangeState<CheckForTurnEndState>();
        }
        else
        {
            Debug.Log("Select a valid tile.");
        }
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null || !tile.isWalkable)
            return;

        if(moveRange.Contains(tile.node))
        {
            List<Node> path = pathfinder.FindPath(player.tile.node, tile.node, player.Stats.moveRange, mover.diag, false, mover.ignoreUnwalkable, false, mover.costModifier);
            if (player.MovementAbility.isPath)
            {
                grid.SelectNodes(path, CustomColors.MovementPath, "movepath");
            }
            else
            {
                grid.SelectNodes(path[path.Count - 1], CustomColors.MovementPath, "movepath");
            }
            
            battleUiController.SetApCost(path[path.Count - 1].gCost, player.Stats.moveRange);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.transform.GetComponent<Tile>();

        if (tile == null)
            return;

        if (moveRange.Contains(tile.node))
        {
            grid.DeSelectNodes("movepath");
            bc.lineRenderer.positionCount = 0;
            battleUiController.SetApCost();
        }
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }

}