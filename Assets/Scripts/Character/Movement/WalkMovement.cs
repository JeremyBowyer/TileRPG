using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMovement : Movement
{

    public override bool diag { get { return false; } set { diag = value; } }
    public override bool isPath { get { return true; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return false; } set { ignoreUnwalkable = value; } }
    public override float Speed { get { return speed; } set { speed = value; } }

    public WalkMovement(Character character, GameController bc) : base(character, bc)
    {
        speed = 2f;
    }

    public override IEnumerator Traverse(Tile tile)
    {
        isMoving = true;
        List<Node> path = gc.pathfinder.FindPath(character.tile.node, tile.node, character.stats.moveRange, diag, false, ignoreUnwalkable);

        foreach (Node node in path)
        {
            float currentTime = 0f;

            float startingX = character.transform.position.x;
            float startingZ = character.transform.position.z;

            float xDist = node.tile.worldPosition.x - character.transform.position.x;
            float zDist = node.tile.worldPosition.z - character.transform.position.z;

            while (!Mathf.Approximately(currentTime, 1.0f))
            {
                currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * Speed));
                float frameValue = (endValue - startValue) * EasingEquations.EaseInOutQuad(0.0f, 1.0f, currentTime) + startValue;
                float newX = startingX + xDist * frameValue;
                float newZ = startingZ + zDist * frameValue;
                character.transform.position = new Vector3(newX, node.worldPosition.y + character.gameObject.GetComponent<BoxCollider>().bounds.extents.y, newZ);
                yield return new WaitForEndOfFrame();
            }
        }
        isMoving = false;
        yield break;
    }
}