using UnityEngine;
using System.Collections;

public class SpellTargetSequenceState : BattleState
{
    public static Character targetCharacter;
    public static TargetSpellAbility spell;
    private Character character;
    private IEnumerator spellCoroutine;

    public override void Enter()
    {
        inTransition = true;
        spell = args.spell as TargetSpellAbility;
        targetCharacter = args.targetCharacter;
        base.Enter();

        character = args.character;
        character.CastSpell(spell);
        spellCoroutine = spell.Initiate(targetCharacter, OnCoroutineFinish);
        StartCoroutine(spellCoroutine);
    }

    public void OnCoroutineFinish()
    {
        targetCharacter.Damage(spell.AbilityPower);
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
        targetCharacter.Damage(spell.AbilityPower);
        character.transform.rotation = Quaternion.LookRotation(gc.grid.GetDirection(character.tile.node, targetCharacter.tile.node), Vector3.up);
        character.animParamController.SetBool("idle", true);
        inTransition = false;
    }

}