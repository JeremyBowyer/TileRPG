using UnityEngine;
using System.Collections;

public class TeleportMovement : Movement
{

    public override bool diag { get { return false; } set { diag = value; } }

    public override IEnumerator Traverse(Tile tile)
    {
        pathfinder.FindPath(transform.position, tile.transform.position, character.stats.moveRange, false, false);
        character.Place(tile, tile.node.gCost);
        if (character.stats.curAP <= 0)
            nextTurn = true;
        yield return null;
    }
}