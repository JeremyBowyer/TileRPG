using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelectUnitState : BattleState
{

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(DishOutDamageState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        bc.NextPlayer();
        InTransition = false;
        bc.ChangeState<DishOutDamageState>();
    }
}