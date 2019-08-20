using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyTurnState : AIBattleState
{
    public override bool IsInterruptible
    {
        get { return false; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(AttackState),
            typeof(HealAllyState),
            typeof(HealSelfState),
            typeof(HostileTargetSpellState),
            typeof(ChaseState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        aiAction.text = "Thinking...";
        StartCoroutine(DelayAction(2f, DecideAction));
    }

    public IEnumerator DelayAction(float secs, Action callback)
    {
        yield return new WaitForSeconds(secs);
        callback?.Invoke();
        yield break;
    }

    public void DecideAction()
    {
        enemyAI.ConsiderOptions();
        Exit();
    }

    public override void Exit()
    {
        base.Exit();
    }
}