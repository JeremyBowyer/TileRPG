using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMovement : Movement
{

    public override bool diag { get { return false; } set { diag = value; } }
    public override bool isPath { get { return true; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return false; } set { ignoreUnwalkable = value; } }
    public override bool ignoreOccupant { get { return false; } set { ignoreOccupant = value; } }
    public override bool ignoreMoveBlock { get { return false; } set { ignoreMoveBlock = value; } }
    public override float Speed { get { return speed; } set { speed = value; } }

    public WalkMovement(Character _character) : base(_character)
    {
        Speed = 4f;
        mName = "Walk";
        mDescription = "Travel to your destination on foot.";
    }

    public override IEnumerator Traverse(List<Node> path, Action callback)
    {
        // Set Animation
        controller.animParamController.SetBool("walking", true);

        // Movement routine
        foreach (Node node in path)
        {
            float currentTime = 0f;

            float startingX = controller.transform.position.x;
            float startingZ = controller.transform.position.z;

            float xDist = node.tile.WorldPosition.x - controller.transform.position.x;
            float zDist = node.tile.WorldPosition.z - controller.transform.position.z;

            float _height = node.worldPosition.y;

            controller.transform.LookAt(new Vector3(node.tile.WorldPosition.x, controller.transform.position.y, node.tile.WorldPosition.z));
            while (!Mathf.Approximately(currentTime, 1.0f))
            {
                currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * Speed));
                float frameValue = (endValue - startValue) * EasingEquations.Linear(0.0f, 1.0f, currentTime) + startValue;
                float newX = startingX + xDist * frameValue;
                float newZ = startingZ + zDist * frameValue;
                float newY = bc.grid.FindHeightPoint(new Vector3(newX, controller.transform.position.y, newZ));
                controller.transform.position = new Vector3(newX, newY, newZ);
                yield return new WaitForEndOfFrame();
            }
            if(node != path[path.Count - 1])
                controller.PassThrough(node.tile);
        }

        // Clean up
        controller.animParamController.SetBool("idle", true);
        callback();
        yield break;
    }
}