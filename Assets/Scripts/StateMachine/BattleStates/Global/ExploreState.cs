using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExploreState : BattleState
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
        inTransition = true;
        base.Enter();
        inTransition = false;
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Debug.Log("Explore State Click");
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

}