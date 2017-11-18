using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTargetState : BattleState
{
    List<Tile> tiles;
    List<Node> moveRange;

    public override void Enter()
    {
        base.Enter();
        Movement mover = owner.currentCharacter.gameObject.GetComponent<TeleportMovement>();
        Player player = owner.currentCharacter as Player;
        moveRange = mover.GetNodesInRange(player.stats.moveRange);
        grid.SelectTiles(moveRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectTiles(moveRange);
        moveRange = null;
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