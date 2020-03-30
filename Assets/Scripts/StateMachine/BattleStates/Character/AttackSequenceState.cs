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
        character.onAttackLand += OnAttackEnd;
        StartCoroutine(attackAbility.Initiate(targetCharacter));
    }

    public void OnAttackEnd(string _clipName)
    {
        attackAbility.ApplyCharacterEffect(targetCharacter);
        callback?.Invoke();
        character.onAttackLand -= OnAttackEnd;
        InTransition = false;
        character.ChangeState<IdleState>();
    }

}