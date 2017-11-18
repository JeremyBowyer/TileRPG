using UnityEngine;
using System.Collections;

public class TeleportMovement : Movement
{
    public override IEnumerator Traverse(Tile tile)
    {
        Node node = tile.node;
        pathfinder.FindPath(transform.position, tile.transform.position, character.stats.moveRange);
        character.Place(tile, tile.node.gCost);
        yield return null;
    }
}