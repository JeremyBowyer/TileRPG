using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellEnvironmentSequenceState : BattleState
{
    public static Character targetCharacter;
    public static Character character;
    public static Tile targetTile;
    private List<Node> splashZone;
    public static EnvironmentSpellAbility spell;

    private IEnumerator spellCoroutine;

    public override void Enter()
    {
        inTransition = true;
        character = args.character;
        spell = args.spell as EnvironmentSpellAbility;
        targetTile = args.targetTile;
        splashZone = args.splashZone;
        base.Enter();

        character = args.character;
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
                node.tile.occupant.GetComponent<Character>().Damage(spell.AbilityPower);
            }
        }
        inTransition = false;
        if (character.NextTurn)
        {
            gc.ChangeState<SelectUnitState>();
            return;
        }
        else
        {
            if (character is Player)
            {
                gc.ChangeState<CommandSelectionState>();
                return;
            }
            else if (character is Enemy)
            {
                gc.ChangeState<EnemyTurnState>();
                return;
            }
        }
    }

    public override void InterruptTransition()
    {
        StopCoroutine(spellCoroutine);
        foreach (Node node in splashZone)
        {
            if (node.tile.occupant != null)
            {
                node.tile.occupant.GetComponent<Character>().Damage(spell.AbilityPower);
            }
        }
        character.animParamController.SetBool("idle", true);
        inTransition = false;
    }
}