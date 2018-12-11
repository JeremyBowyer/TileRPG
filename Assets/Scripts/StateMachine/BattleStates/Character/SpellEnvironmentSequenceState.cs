using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellEnvironmentSequenceState : BattleState
{
    public static CharacterController targetCharacter;
    public static CharacterController character;
    public static Tile targetTile;
    private List<Node> splashZone;
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
        character = GetComponent<CharacterController>();
        spell = args.spell as EnvironmentSpellAbility;
        targetTile = args.targetTile;
        splashZone = args.splashZone;
        base.Enter();

        character.CastSpell(spell);
        spellCoroutine = spell.Initiate(targetTile, OnCoroutineFinish);
        StartCoroutine(spellCoroutine);
    }

    public void OnCoroutineFinish()
    {
        foreach (Node node in splashZone)
        {
            if (node.tile.occupant != null)
            {
                node.tile.occupant.GetComponent<CharacterController>().Damage(spell.AbilityPower);
            }
        }
        inTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        StopCoroutine(spellCoroutine);
        foreach (Node node in splashZone)
        {
            if (node.tile.occupant != null)
            {
                node.tile.occupant.GetComponent<CharacterController>().Damage(spell.AbilityPower);
            }
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