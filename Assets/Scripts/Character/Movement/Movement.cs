using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Movement : MonoBehaviour
{
    public int limit;
    protected Character character;
    protected Pathfinding pathfinder;
    public bool nextTurn;
    public abstract bool diag { get; set; }

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinding>();
    }

    public virtual List<Node> GetNodesInRange(int limit, bool diag)
    {
        List<Node> retValue = pathfinder.FindRange(transform.position, limit, diag);
        return retValue;
    }

    public abstract IEnumerator Traverse(Tile tile);


}