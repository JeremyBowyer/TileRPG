using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Movement
{
    public int limit;
    public float speed = 1f;
    public float startValue = 0f;
    public float endValue = 1f;
    protected CharController controller;
    protected GameController gc
    {
        get { return controller.gc; }
    }
    protected Pathfinding pathfinder;
    public abstract bool diag { get; set; }
    public abstract bool ignoreUnwalkable { get; set; }
    public abstract bool ignoreOccupant { get; set; }
    public abstract bool isPath { get; set; }
    public abstract float Speed { get; set; }

    public Movement(CharController _character)
    {
        controller = _character;
    }

    public virtual List<Node> GetNodesInRange(int limit, bool diag, bool ignoreOccupant)
    {
        List<Node> retValue = gc.pathfinder.FindRange(controller.tile.node, limit, diag, ignoreOccupant, ignoreUnwalkable, false);
        return retValue;
    }

    //public abstract IEnumerator Traverse(Tile startingTile, Tile targetTile, Action callback);
    public abstract IEnumerator Traverse(List<Node> path, Action callback);


}