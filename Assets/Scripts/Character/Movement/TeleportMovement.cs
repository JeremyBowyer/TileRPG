using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TeleportMovement : Movement
{

    public override bool diag { get { return true; } set { diag = value; } }
    public override bool isPath { get { return false; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return true; } set { ignoreUnwalkable = value; } }
    public override bool ignoreOccupant{ get { return true; } set { ignoreOccupant = value; } }
    public override float Speed { get { return 1 / speed; } set { speed = value; } }

    public TeleportMovement(CharController controller) : base(controller)
    {
        mName = "Teleport";
    }

    public override IEnumerator Traverse(List<Node> path, Action callback)
    {
        Tile targetTile = path[path.Count - 1].tile;
        Vector3 _targetPos = targetTile.transform.position;
        controller.transform.LookAt(new Vector3(targetTile.transform.position.x, controller.transform.position.y, targetTile.transform.position.z));
        controller.transform.position = _targetPos;
        callback();
        yield break;
    }
}