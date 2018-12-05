using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySequence : BattleState
{

    public override bool isInterruptable
    {
        get { return false; }
    }
    public override bool isMaster
    {
        get { return true; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(WorldExploreState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.grid.ClearGrid();
        gc.protag.transform.position = gc.protagStartPos;
        gc.battleCharacters.Clear();
        inTransition = false;
        gc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
