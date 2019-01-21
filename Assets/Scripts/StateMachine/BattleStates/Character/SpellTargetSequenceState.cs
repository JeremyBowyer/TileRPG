using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpellTargetSequenceState : BattleState
{
    public static CharController targetCharacter;
    public static TargetSpellAbility spell;
    private CharController character;
    private State stateToNotify;
    private IEnumerator spellCoroutine;
    private Action callback;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(CommandSelectionState),
            typeof(EnemyTurnState),
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
        spell = args.spell as TargetSpellAbility;
        targetCharacter = args.targetCharacter;
        callback = args.callback;
        base.Enter();

        character = GetComponent<CharController>();
        character.CastSpell(spell);
        spellCoroutine = spell.Initiate(targetCharacter, OnCoroutineFinish);
        StartCoroutine(spellCoroutine);
    }

    public void OnCoroutineFinish()
    {
        spell.ApplyEffect(targetCharacter);
        if (callback != null)
            callback();
        inTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        StopCoroutine(spellCoroutine);
        spell.ApplyEffect(targetCharacter);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("SpellTargetGO"))
        {
            GameObject.Destroy(go);
        }
        character.transform.rotation = Quaternion.LookRotation(gc.grid.GetDirection(character.tile.node, targetCharacter.tile.node), Vector3.up);
        character.animParamController.SetBool("idle", true);
        if (callback != null)
            callback();
        isInterrupting = false;
        inTransition = false;
        character.ChangeState<IdleState>();
    }

}