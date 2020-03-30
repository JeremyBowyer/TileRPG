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
            typeof(DeathSequence),
            typeof(UnitTurnState),
            typeof(EnemyTurnState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
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
        spell.ApplyCharacterEffect(targetCharacter);
        if (callback != null)
            callback();
        InTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition(bool finish)
    {
        isInterrupting = true;
        StopCoroutine(spellCoroutine);
        spell.ApplyCharacterEffect(targetCharacter);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("SpellTargetGO"))
        {
            GameObject.Destroy(go);
        }
        character.transform.rotation = Quaternion.LookRotation(bc.grid.GetDirection(character.tile.node, targetCharacter.tile.node), Vector3.up);
        character.animParamController.SetBool("idle", true);
        if (callback != null)
            callback();
        isInterrupting = false;
        InTransition = false;
        character.ChangeState<IdleState>();
    }
}