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
        base.Enter();
        mover = owner.currentCharacter.gameObject.GetComponent<TeleportMovement>();
        player = owner.currentCharacter as Player;
        moveRange = mover.GetNodesInRange(player.stats.moveRange, mover.diag, false);
        grid.SelectRange(moveRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectRange(moveRange);
        moveRange = null;
        uiController.SetApCost();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Map");
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        if (e.info.GetComponent<Tile>())
        {
            if (moveRange.Contains(e.info.GetComponent<Tile>().node))
            {
                owner.currentTile = e.info.GetComponent<Tile>();
                owner.ChangeState<MoveSequenceState>();
            }
            else
            {
                Debug.Log("Select a valid tile.");
            }
        }
        else
        {
            Debug.Log("Select a tile.");
        }
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        if(moveRange.Contains(e.info.gameObject.GetComponent<Tile>().node))
        {
            List<Node> path = pathfinder.FindPath(player.transform.position, e.info.gameObject.transform.position, player.stats.moveRange, mover.diag, false);
            grid.SelectPath(path);
            uiController.SetApCost(path[path.Count - 1].gCost, player.stats.moveRange);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        if (moveRange.Contains(e.info.gameObject.GetComponent<Tile>().node))
        {
            grid.DeSelectPath(pathfinder.FindPath(player.transform.position, e.info.gameObject.transform.position, player.stats.moveRange, mover.diag, false));
            uiController.SetApCost();
        }
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
        {
            if (tiles.Contains(owner.currentTile))
                owner.ChangeState<MoveSequenceState>();
        }
        else
        {
            owner.ChangeState<CommandSelectionState>();
        }
    }

}