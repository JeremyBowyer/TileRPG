using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyTurnStateGlobal : BattleState
{
    private IEnumerator enemyTurnCoroutine;
    private BaseAI enemyAI;
    private bool isThinking;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(MoveSequenceState),
            typeof(AttackSequenceState),
            typeof(SpellEnvironmentSequenceState),
            typeof(SpellTargetSequenceState),
            typeof(SelectUnitState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        enemyAI = bc.CurrentCharacter.GetComponent<BaseAI>();
    }


    public void OnDecide()
    {
        bool endTurn = enemyAI.EndOfTurn;
        if (endTurn)
        {
            InTransition = false;
            bc.ChangeState<SelectUnitState>();
        }
        else
        {
            StopCoroutine(enemyTurnCoroutine);
            StartCoroutine(enemyTurnCoroutine);
        }
    }

    public override void InterruptTransition()
    {
        StopCoroutine(enemyTurnCoroutine);
        InTransition = false;
    }

    public override void Exit()
    {
        base.Exit();

    }

}