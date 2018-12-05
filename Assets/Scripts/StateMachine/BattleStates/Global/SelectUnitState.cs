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
            typeof(CommandSelectionState),
            typeof(EnemyTurnState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.NextPlayer();
        if (gc.currentCharacter is Player)
        {
            inTransition = false;
            gc.ChangeState<CommandSelectionState>();
            return;
        }
        else if (gc.currentCharacter is Enemy)
        {
            inTransition = false;
            gc.ChangeState<EnemyTurnState>();
            return;
        }
        inTransition = false;
    }
}