using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{

    public override bool IsInterruptible
    {
        get { return true; }
    }

    public override void Enter()
    {
        base.Enter();
        InTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
