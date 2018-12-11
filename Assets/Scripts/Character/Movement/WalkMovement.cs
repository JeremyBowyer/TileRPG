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
    public override float Speed { get { return speed; } set { speed = value; } }

    public WalkMovement(CharacterController character, GameController bc) : base(character, bc)
    {
        speed = 4f;
    }

    public override IEnumerator Traverse(List<Node> path, Action callback)
    {
        // Set Animation
        character.animParamController.SetBool("running", true);

        // Movement routine
        foreach (Node node in path)
        {
            float currentTime = 0f;

            float startingX = character.transform.position.x;
            float startingZ = character.transform.position.z;

            float xDist = node.tile.worldPosition.x - character.transform.position.x;
            float zDist = node.tile.worldPosition.z - character.transform.position.z;

            float _height = node.worldPosition.y + character.height;

            character.transform.LookAt(new Vector3(node.tile.transform.position.x, character.transform.position.y, node.tile.transform.position.z));
            while (!Mathf.Approximately(currentTime, 1.0f))
            {
                currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * Speed));
                float frameValue = (endValue - startValue) * EasingEquations.EaseInOutQuad(0.0f, 1.0f, currentTime) + startValue;
                float newX = startingX + xDist * frameValue;
                float newZ = startingZ + zDist * frameValue;
                character.transform.position = new Vector3(newX, _height, newZ);
                yield return new WaitForEndOfFrame();
            }
        }

        // Clean up
        character.animParamController.SetBool("idle", true);
        callback();
        yield break;
    }
}