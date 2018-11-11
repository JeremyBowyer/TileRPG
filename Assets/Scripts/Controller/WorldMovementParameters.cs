using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldMovementParameters
{
    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    public Vector3 MaxVelocity = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeLimit = 30;
    public float Gravity = -5f;

    public JumpBehavior JumpRestrictions;

    public float JumpFrequency = 0.25f;

    [SerializeField]
    public float TimeForFullJump = 0.25f;
    [SerializeField]
    public float MinJumpForce = 2.0f;
    [SerializeField]
    public float MaxJumpForce = 12.0f;
    [SerializeField]
    public float HorizontalJumpForce = 12.0f;
}
