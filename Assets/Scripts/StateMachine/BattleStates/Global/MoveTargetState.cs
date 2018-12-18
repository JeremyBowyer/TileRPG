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
        mover = gc.currentCharacter.MovementAbility;
        player = gc.currentCharacter as PlayerController;
        moveRange = mover.GetNodesInRange(player.Stats.moveRange, mover.diag, false);
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
            List<Node> path = gc.pathfinder.FindPath(
                gc.currentCharacter.tile.node,
                tile.node,
                player.Stats.moveRange,
                player.MovementAbility.diag,
                player.MovementAbility.ignoreOccupant,
                player.MovementAbility.ignoreUnwalkable,
                false);

            StateArgs moveArgs = new StateArgs
            {
                path = path,
                waitingStateMachines = new List<StateMachine> { gc }
            };
            gc.currentCharacter.ChangeState<MoveSequenceState>(moveArgs);
            gc.ChangeState<CheckForTurnEndState>();
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

        if(moveRange.Contains(tile.node))
        {
            List<Node> path = pathfinder.FindPath(player.tile.node, tile.node, player.Stats.moveRange, mover.diag, false, mover.ignoreUnwalkable, false);
            if (player.MovementAbility.isPath)
            {
                grid.SelectNodes(path, Color.black);
            }
            else
            {
                grid.SelectNodes(path[path.Count - 1], Color.black);
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
            grid.DeSelectNodes();
            battleUiController.SetApCost();
        }
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

}