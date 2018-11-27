using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTargetState : BattleState
{
    List<Tile> tiles;
    List<Node> moveRange;
    Player player;
    Movement mover;


    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        mover = gc.currentCharacter.movementAbility;
        player = gc.currentCharacter as Player;
        moveRange = mover.GetNodesInRange(player.stats.moveRange, mover.diag, false);
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
        uiController.SetApCost();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("GridClick");
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.Log("Select a tile.");
            return;
        }

        if (moveRange.Contains(tile.node))
        {
            List<Node> path = gc.pathfinder.FindPath(gc.currentCharacter.tile.node, tile.node, player.stats.moveRange, player.movementAbility.diag, player.movementAbility.ignoreOccupant, player.movementAbility.ignoreUnwalkable, false);
            StateArgs moveArgs = new StateArgs
            {
                path = path,
                character = gc.currentCharacter
            };
            gc.ChangeState<MoveSequenceState>(moveArgs);
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
            List<Node> path = pathfinder.FindPath(player.tile.node, tile.node, player.stats.moveRange, mover.diag, false, mover.ignoreUnwalkable, false);
            if (player.movementAbility.isPath)
            {
                grid.SelectNodes(path, Color.black);
            }
            else
            {
                grid.SelectNodes(path[path.Count - 1], Color.black);
            }
            
            uiController.SetApCost(path[path.Count - 1].gCost, player.stats.moveRange);
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
            uiController.SetApCost();
        }
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
        {
            if (tiles.Contains(gc.currentTile))
                gc.ChangeState<MoveSequenceState>();
        }
        else
        {
            gc.ChangeState<CommandSelectionState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

}