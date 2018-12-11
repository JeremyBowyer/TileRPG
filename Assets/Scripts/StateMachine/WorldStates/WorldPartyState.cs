using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPartyState : WorldBaseMenuState
{

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(WorldMenuState)
            };
        }
        set { }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<WorldMenuState>();
    }
}
