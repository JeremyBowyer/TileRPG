using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AIBattleState
{
    private CharController target;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(MoveSequenceState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        target = args.targetCharacter;

        List<Node> path = character.AttackAbility.PathToGetInRange(target);
        Debug.Log("starting chase");
        if (path.Count == 0)
        {
            Debug.Log("couldn't find path from " + character.name + " to " + target.name);
            character.ChangeState<IdleState>();
            bc.ChangeState<SelectUnitState>();
            InTransition = false;
        }
        else
        {
            Debug.Log("found path");
            aiAction.text = "Chasing...";
            StateArgs moveArgs = new StateArgs
            {
                path = path,
                waitingStateMachines = new List<StateMachine> { bc },
                callback = InitiateTurn
            };
            character.ChangeState<MoveSequenceState>(moveArgs);
            InTransition = false;
        }

    }
}
