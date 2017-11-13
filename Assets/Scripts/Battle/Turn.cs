using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turn
{
    public Character actor;
    public bool lockMove;
    Tile startTile;

    public void Change(Character current)
    {
        actor = current;
        lockMove = false;
        startTile = actor.tile;
    }

}