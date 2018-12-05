using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{

    public override bool isInterruptable
    {
        get { return true; }
    }

    public override void Enter()
    {
        base.Enter();
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
