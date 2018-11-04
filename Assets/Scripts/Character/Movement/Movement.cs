using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Movement
{
    public int limit;
    public float speed = 1f;
    public float startValue = 0f;
    public float endValue = 1f;
    protected Character character;
    protected GameController gc;
    protected Pathfinding pathfinder;
    public bool nextTurn;
    public abstract bool diag { get; set; }
    public abstract bool ignoreUnwalkable { get; set; }
    public abstract bool isPath { get; set; }
    public abstract float Speed { get; set; }
    public bool isMoving;

    public Movement(Character _character, GameController _gc)
    {
        character = _character;
        gc = _gc;
    }

    public virtual List<Node> GetNodesInRange(int limit, bool diag, bool ignoreOccupant)
    {
        List<Node> retValue = gc.pathfinder.FindRange(character.tile.node, limit, diag, ignoreOccupant, ignoreUnwalkable);
        return retValue;
    }

    public abstract IEnumerator Traverse(Tile tile);


}