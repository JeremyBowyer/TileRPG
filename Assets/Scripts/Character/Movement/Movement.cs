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
    public float costModifier = 1f;
    public string mName;
    protected CharController controller;
    protected BattleController bc
    {
        get { return controller.bc; }
    }
    protected Pathfinding pathfinder
    {
        get { return bc.pathfinder; }
    }
    public abstract bool diag { get; set; }
    public abstract bool ignoreUnwalkable { get; set; }
    public abstract bool ignoreOccupant { get; set; }
    public abstract bool ignoreMoveBlock { get; set; }
    public abstract bool isPath { get; set; }
    public abstract float Speed { get; set; }

    public Movement(CharController _character)
    {
        controller = _character;
    }

    public virtual List<Node> GetNodesInRange()
    {
        List<Node> moveRange = pathfinder.FindRange(
            controller.tile.node,
            controller.Stats.moveRange,
            diag,
            ignoreOccupant,
            ignoreUnwalkable,
            false,
            ignoreMoveBlock,
            costModifier);

        moveRange = bc.pathfinder.CullNodes(moveRange, true, true);
        return moveRange;
    }

    public virtual List<Node> GetPath(Node node)
    {
        List<Node> path = pathfinder.FindPath(
            controller.tile.node,
            node,
            controller.Stats.moveRange,
            diag,
            ignoreOccupant,
            ignoreUnwalkable,
            false,
            ignoreMoveBlock,
            costModifier);

        path = bc.pathfinder.CullNodes(path, true, true);

        return path;
    }

    public abstract IEnumerator Traverse(List<Node> path, Action callback);

}