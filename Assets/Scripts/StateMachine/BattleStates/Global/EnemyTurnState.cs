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
        enemyAI = bc.CurrentCharacter.GetComponent<BaseAI>();
        enemyTurnCoroutine = ConsiderOptions();
        StartCoroutine(enemyTurnCoroutine);
    }

    public IEnumerator ConsiderOptions()
    {
        enemyAI.aiAction.text = "Thinking...";
        yield return new WaitForSeconds(2f);
        enemyAI.ConsiderOptions(OnDecide);
        yield break;
    }

    public void OnDecide()
    {
        bool endTurn = enemyAI.EndOfTurn;
        if (endTurn)
        {
            inTransition = false;
            bc.ChangeState<SelectUnitState>();
        }
        else
        {
            StopCoroutine(enemyTurnCoroutine);
            enemyTurnCoroutine = ConsiderOptions();
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