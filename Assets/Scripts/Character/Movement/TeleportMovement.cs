using UnityEngine;
using System.Collections;

public class TeleportMovement : Movement
{

    public override bool diag { get { return true; } set { diag = value; } }
    public override bool isPath { get { return false; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return true; } set { ignoreUnwalkable = value; } }
    public override float Speed { get { return 1 / speed; } set { speed = value; } }

    public TeleportMovement(Character _character, GameController _bc) : base(_character, _bc)
    {

    }

    public override IEnumerator Traverse(Tile tile)
    {
        isMoving = true;
        Vector3 _targetPos = tile.transform.position + new Vector3(0, character.height, 0);
        character.transform.LookAt(new Vector3(tile.transform.position.x, character.transform.position.y, tile.transform.position.z));
        character.transform.position = _targetPos;
        isMoving = false;
        yield return null;
        yield break; ;
    }
}