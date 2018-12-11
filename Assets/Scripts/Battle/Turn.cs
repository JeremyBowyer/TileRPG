using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turn
{
    public CharacterController actor;
    public bool lockMove;
    public Tile startTile;

    public void Change(CharacterController current)
    {
        actor = current;
        lockMove = false;
        startTile = actor.tile;
    }

}