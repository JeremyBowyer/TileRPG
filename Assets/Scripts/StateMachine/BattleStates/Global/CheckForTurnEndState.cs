using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CheckForTurnEndState : BattleState
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
        if(gc.currentCharacter.NextTurn)
            gc.NextPlayer();
        if (gc.currentCharacter is PlayerController)
        {
            inTransition = false;
            gc.ChangeState<CommandSelectionState>();
            return;
        }
        else if (gc.currentCharacter is EnemyController)
        {
            inTransition = false;
            gc.ChangeState<EnemyTurnState>();
            return;
        }
        inTransition = false;
    }
}