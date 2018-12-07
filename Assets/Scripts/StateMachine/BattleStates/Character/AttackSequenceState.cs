using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackSequenceState : BattleState
{
    Character character;
    Character targetCharacter;
    Action callback;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(EnemyTurnState),
            typeof(CommandSelectionState),
            typeof(VictorySequence),
            typeof(DeathSequence),
            typeof(CheckForTurnEndState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        character = GetComponent<Character>();
        targetCharacter = args.targetCharacter;
        callback = args.callback;
        character.Attack(targetCharacter, character.attackAbility);
        StartCoroutine(character.attackAbility.Initiate(targetCharacter, OnAttackEnd));
    }

    public void OnAttackEnd()
    {
        targetCharacter.Damage(character.attackAbility.AbilityPower);
        if (callback != null)
            callback();
        inTransition = false;
        character.ChangeState<IdleState>();
    }

}