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
        Node frontNode = bc.grid.FindNearestNode(bc.protagStartPos);
        character.Place(frontNode.tile);
        character.InitBattle();
        bc.FollowTarget(character.transform);
        battleUI.LoadCurrentStats(character);
        //bc.CurrentCharacter = character;

        mover = new PlaceUnitMovement(character.character);
        moveRange = mover.GetNodesInRange();
        grid.SelectNodes(moveRange, CustomColors.ChangeAlpha(CustomColors.MovementRange, 0.25f), "moverange", "empty");
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

            character.Place(tile);
            character.gameObject.transform.localScale = Vector3.one / 2;

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
        OutlineTargetCharacter(sender, e);

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
        RemoveOutlineTargetCharacter();

        Tile tile = e.info.gameObject.transform.GetComponent<Tile>();

        if (tile == null)
            return;

        if (moveRange.Contains(tile.node))
        {
            grid.DeSelectNodes("movepath");
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