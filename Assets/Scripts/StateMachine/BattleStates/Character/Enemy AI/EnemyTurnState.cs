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
        if (aiAction == null)
            Debug.Log("hi");
        aiAction.text = "Thinking...";
        StartCoroutine(DelayAction(3f, DecideAction));
    }

    public IEnumerator DelayAction(float secs, Action callback)
    {
        Debug.Log("starting");
        yield return new WaitForSeconds(secs);
        if (callback != null)
            callback();
        yield break;
    }

    public void DecideAction()
    {
        Debug.Log("considering");
        enemyAI.ConsiderOptions();
        Debug.Log("considered");
        Exit();
    }

    public override void Exit()
    {
        base.Exit();
    }

}