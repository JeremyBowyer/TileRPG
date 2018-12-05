using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyTurnState : BattleState
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
        inTransition = true;
        base.Enter();
        enemyAI = gc.currentCharacter.GetComponent<BaseAI>();
        StartCoroutine(Countdown());
        enemyTurnCoroutine = ConsiderOptions();
        StartCoroutine(enemyTurnCoroutine);
    }

    public IEnumerator Countdown()
    {
        isThinking = true;
        int cnt = 2;
        enemyAI.aiAction.text = "Thinking...";
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1f);
            cnt--;
        }
        isThinking = false;
    }

    public IEnumerator ConsiderOptions()
    {
        while (isThinking)
        {
            yield return null;
        }
        enemyAI.ConsiderOptions(OnDecide);
        yield break;
    }

    public void OnDecide()
    {
        bool endTurn = enemyAI.EndOfTurn;
        if (endTurn)
        {
            inTransition = false;
            gc.ChangeState<SelectUnitState>();
        }
        else
        {
            StartCoroutine(enemyTurnCoroutine);
        }
    }

    public override void InterruptTransition()
    {
        StopCoroutine(enemyTurnCoroutine);
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();

    }

}