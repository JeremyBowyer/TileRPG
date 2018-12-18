using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turn
{
    public CharController actor;
    public bool lockMove;
    public Tile startTile;

    public void Change(CharController current)
    {
        actor = current;
        lockMove = false;
        startTile = actor.tile;
    }

}