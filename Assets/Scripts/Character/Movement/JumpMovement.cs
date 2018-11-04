using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class JumpMovement : Movement
{

    public override bool diag { get { return true; } set { diag = value; } }
    public override bool isPath { get { return false; } set { isPath = value; } }
    public override bool ignoreUnwalkable { get { return true; } set { ignoreUnwalkable = value; } }
    public override float Speed { get { return speed; } set { speed = value; } }

    private float currentTime;
    private float jumpHeight;
    private float jumpSpeed;

    Func<float, float, float, float> jumpEquation;

    private float startingX;
    private float startingY;
    private float startingZ;

    private float xDist;
    private float zDist;

    public JumpMovement(Character _character, GameController _gc) : base(_character, _gc)
    {

    }

    public override IEnumerator Traverse(Tile tile)
    {
        isMoving = true;
        float _height = tile.gameObject.GetComponent<BoxCollider>().bounds.extents.z + character.gameObject.GetComponent<BoxCollider>().bounds.extents.z;
        Vector3 startingPos = character.transform.position;
        Vector3 endingPos = tile.transform.position + new Vector3(0, _height, 0);
        currentTime = 0f;
        jumpHeight = Vector3.Distance(startingPos, endingPos) / 2;
        jumpSpeed = speed + speed / jumpHeight;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * jumpSpeed));
            Vector3 framePos = MathParabola.Parabola(startingPos, endingPos, jumpHeight, currentTime);
            character.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        yield break;
    }

}