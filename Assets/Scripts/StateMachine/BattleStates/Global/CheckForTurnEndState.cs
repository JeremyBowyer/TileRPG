﻿using UnityEngine;
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
        if(bc.CurrentCharacter.NextTurn && bc.CurrentCharacter is EnemyController)
            bc.NextPlayer();
        if (bc.CurrentCharacter is PlayerController)
        {
            inTransition = false;
            bc.ChangeState<CommandSelectionState>();
            return;
        }
        else if (bc.CurrentCharacter is EnemyController)
        {
            inTransition = false;
            bc.ChangeState<EnemyTurnState>();
            return;
        }
        inTransition = false;
    }
}