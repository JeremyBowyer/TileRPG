using UnityEngine;
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
        InTransition = true;
        base.Enter();
        character = args.character;

        bc.FollowTarget(character.transform);

        mover = character.MovementAbility;
        moveRange = mover.GetNodesInRange();
        grid.SelectNodes(moveRange, CustomColors.ChangeAlpha(CustomColors.MovementRange, 0.04f), "moverange", "inner");
        grid.OutlineNodes(moveRange, CustomColors.MovementRange);
        InTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectNodes("moverange");
        grid.DeSelectNodes("movepath");
        grid.RemoveOutline(moveRange);
        moveRange = null;
        battleUI.SetApCost();
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
            List<Node> path = mover.GetPath(tile.node);

            StateArgs moveArgs = new StateArgs
            {
                path = path,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            character.ChangeState<MoveSequenceState>(moveArgs);
            bc.ChangeState<PlaceUnitsState>();
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
            List<Node> path = mover.GetPath(tile.node);
            if (mover.isPath)
            {
                grid.SelectNodes(path, CustomColors.MovementPath, "movepath", "inner");
            }
            else
            {
                grid.SelectNodes(path[path.Count - 1], CustomColors.MovementPath, "movepath", "inner");
            }
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
            battleUI.SetApCost();
        }
    }

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
        cameraRig.ScreenEdgeMovement(e.info.x, e.info.y);
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

}