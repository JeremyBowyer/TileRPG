using UnityEngine;
using System.Collections;

public class TeleportMovement : Movement
{
    public override IEnumerator Traverse(Tile tile)
    {
        character.Place(tile);
        yield return null;
    }
}