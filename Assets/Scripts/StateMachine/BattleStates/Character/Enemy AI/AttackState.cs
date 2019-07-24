using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AIBattleState
{
    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(AttackSequenceState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        aiAction.text = "Attacking...";
        args.callback = InitiateTurn;
        character.ChangeState<AttackSequenceState>(args);
        InTransition = false;
    }
}
