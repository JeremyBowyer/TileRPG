using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExploreBattleState : BattleState
{

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(CommandSelectionState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        bc.cameraRig.isFollowing = false;
        base.Enter();
        InTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Debug.Log("ExploreBattleState click");
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
        cameraRig.ScreenEdgeMovement(e.info.x, e.info.y);
    }

}