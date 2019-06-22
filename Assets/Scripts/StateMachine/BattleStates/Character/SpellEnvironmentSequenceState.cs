using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellEnvironmentSequenceState : BattleState
{
    public static CharController targetCharacter;
    public static CharController character;
    public static Tile targetTile;
    private List<Node> affectedArea;
    public static EnvironmentSpellAbility spell;
    private State stateToNotify;

    private IEnumerator spellCoroutine;

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
            typeof(DeathSequence)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        character = GetComponent<CharController>();
        spell = args.spell as EnvironmentSpellAbility;
        targetTile = args.targetTile;
        affectedArea = args.affectedArea;
        base.Enter();

        character.CastSpell(spell);
        spellCoroutine = spell.Initiate(targetTile, OnCoroutineFinish);
        StartCoroutine(spellCoroutine);
    }

    public void OnCoroutineFinish()
    {
        foreach (Node node in affectedArea)
        {
            spell.ApplyTileEffect(node.tile);
        }
        inTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        StopCoroutine(spellCoroutine);
        foreach (Node node in affectedArea)
        {
            spell.ApplyTileEffect(node.tile);
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("SpellEnvironmentGO"))
        {
            GameObject.Destroy(go);
        }
        character.animParamController.SetBool("idle", true);
        inTransition = false;
        isInterrupting = false;
        character.ChangeState<IdleState>();
    }
}