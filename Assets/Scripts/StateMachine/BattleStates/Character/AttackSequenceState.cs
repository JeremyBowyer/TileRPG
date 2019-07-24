using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackSequenceState : BattleState
{
    CharController character;
    CharController targetCharacter;
    AttackAbility attackAbility;
    Action callback;

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
        targetCharacter = args.targetCharacter;
        attackAbility = args.attackAbility;
        callback = args.callback;
        character.Attack(targetCharacter, attackAbility);
        StartCoroutine(attackAbility.Initiate(targetCharacter, OnAttackEnd));
    }

    public void OnAttackEnd()
    {
        attackAbility.ApplyCharacterEffect(targetCharacter);
        if (callback != null)
            callback();
        InTransition = false;
        character.ChangeState<IdleState>();
    }

}