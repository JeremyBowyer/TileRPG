﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMovement : Movement
{

    public override bool diag { get { return false; } set { diag = value; } }
    public override bool isPath { get { return true; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return false; } set { ignoreUnwalkable = value; } }
    public override bool ignoreOccupant { get { return false; } set { ignoreOccupant = value; } }
    public override float Speed { get { return speed; } set { speed = value; } }

    public WalkMovement(CharController controller) : base(controller)
    {
        speed = 4f;
        mName = "Walk";
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

            controller.transform.LookAt(new Vector3(node.tile.transform.position.x, controller.transform.position.y, node.tile.transform.position.z));
            while (!Mathf.Approximately(currentTime, 1.0f))
            {
                currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * Speed));
                float frameValue = (endValue - startValue) * EasingEquations.Linear(0.0f, 1.0f, currentTime) + startValue;
                float newX = startingX + xDist * frameValue;
                float newZ = startingZ + zDist * frameValue;
                controller.transform.position = new Vector3(newX, _height, newZ);
                yield return new WaitForEndOfFrame();
            }

            controller.OccupyTile(node.tile);
        }

        // Clean up
        controller.animParamController.SetBool("idle", true);
        callback();
        yield break;
    }
}