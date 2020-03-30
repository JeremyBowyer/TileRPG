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

        if (path.Count == 0)
        {
            path = character.MovementAbility.GetPath(target.tile.node);
            Debug.Log("Couldn't get in range, going straight for target instead");
        }

        if (path.Count == 0)
        {
            Debug.Log("couldn't find path from " + character.name + " to " + target.name);
            character.ChangeState<IdleState>();
            bc.ChangeState<SelectUnitState>();
            InTransition = false;
        }
        else
        {
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
