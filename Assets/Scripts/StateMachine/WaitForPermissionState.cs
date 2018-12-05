using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPermissionState : State
{

    private bool permitted;

    public override void Enter()
    {
        base.Enter();
        inTransition = true;
    }

    public override void Permit(bool _permitted)
    {
        inTransition = !_permitted;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
