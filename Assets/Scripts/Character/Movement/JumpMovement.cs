﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class JumpMovement : Movement
{

    public override bool diag { get { return true; } set { diag = value; } }
    public override bool isPath { get { return false; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return true; } set { ignoreUnwalkable = value; } }
    public override bool ignoreOccupant { get { return false; } set { ignoreOccupant = value; } }
    public override float Speed { get { return speed; } set { speed = value; } }

    private float currentTime;
    private float jumpHeight;
    private float jumpSpeed;

    public JumpMovement(CharacterController _character, GameController _gc) : base(_character, _gc)
    {

    }

    public override IEnumerator Traverse(List<Node> path, Action callback)
    {
        character.animParamController.SetBool("falling", true);

        Tile targetTile = path[path.Count - 1].tile;
        Vector3 startingPos = character.transform.position;
        Vector3 endingPos = targetTile.transform.position + new Vector3(0, character.height, 0);
        //character.transform.LookAt(new Vector3(tile.transform.position.x, character.transform.position.y, tile.transform.position.z));
        character.transform.rotation = Quaternion.LookRotation(gc.grid.GetDirection(path[0], targetTile.node), Vector3.up);
        currentTime = 0f;
        jumpHeight = Vector3.Distance(startingPos, endingPos) / 2;
        jumpSpeed = speed + speed / jumpHeight;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * jumpSpeed));
            Vector3 framePos = MathCurves.Parabola(startingPos, endingPos, jumpHeight, currentTime);
            character.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }

        // Clean up
        character.animParamController.SetBool("idle", true);
        callback();
        yield break;
    }

}