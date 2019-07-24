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
            typeof(EnemyTurnStateGlobal)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        if(bc.CurrentCharacter.NextTurn && bc.CurrentCharacter is EnemyController)
            bc.NextPlayer();
        if (bc.CurrentCharacter is PlayerController)
        {
            InTransition = false;
            bc.ChangeState<CommandSelectionState>();
            return;
        }
        else if (bc.CurrentCharacter is EnemyController)
        {
            InTransition = false;
            bc.ChangeState<EnemyTurnStateGlobal>();
            return;
        }
        InTransition = false;
    }
}