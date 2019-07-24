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
            typeof(IdleState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
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
            bc.CurrentCharacter.ChangeState<EnemyTurnState>();
            bc.ChangeState<IdleState>();
            return;
        }
        InTransition = false;
    }
}