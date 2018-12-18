using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackSequenceState : BattleState
{
    CharController character;
    CharController targetCharacter;
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
        character = GetComponent<CharController>();
        targetCharacter = args.targetCharacter;
        callback = args.callback;
        character.Attack(targetCharacter, character.AttackAbility);
        StartCoroutine(character.AttackAbility.Initiate(targetCharacter, OnAttackEnd));
    }

    public void OnAttackEnd()
    {
        targetCharacter.Damage(character.AttackAbility.AbilityPower);
        if (callback != null)
            callback();
        inTransition = false;
        character.ChangeState<IdleState>();
    }

}