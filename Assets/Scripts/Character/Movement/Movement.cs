using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Movement : MonoBehaviour
{
    public int limit;
    protected Character character;
    protected Pathfinding pathfinder;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinding>();
    }

    public virtual List<Node> GetNodesInRange(int limit)
    {
        List<Node> retValue = pathfinder.FindRange(transform.position, limit);
        return retValue;
    }

    public abstract IEnumerator Traverse(Tile tile);


}