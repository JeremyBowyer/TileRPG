using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedSequenceState : BattleState
{
    private CharController character;
    private Action callback;

    private IEnumerator damagedCoroutine;

    public override bool IsInterruptible
    {
        get { return true; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(DeathSequence),
            typeof(EnemyTurnState),
            typeof(IdleState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        character = GetComponent<CharController>();
        bc.FollowTarget(character.transform);
        callback = args.callback;

        StartCoroutine(damagedCoroutine);
    }

    public void OnCoroutineFinish()
    {
        InTransition = false;
        callback?.Invoke();
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition(bool finish)
    {
        StopCoroutine(damagedCoroutine);

        character.animParamController.SetBool("idle", true);
        OnCoroutineFinish();
    }
}
