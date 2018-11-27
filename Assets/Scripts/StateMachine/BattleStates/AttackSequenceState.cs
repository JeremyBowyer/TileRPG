using UnityEngine;
using System.Collections;
public class AttackSequenceState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine("Sequence");
    }

    IEnumerator Sequence()
    {
        Character character = gc.currentCharacter;
        character.Attack(character.attackTarget, character.attackAbility);
        while (character.attackAbility.inProgress)
            yield return null;
        while (gc._inTransition)
            yield return null;
        if (character.NextTurn)
        {
            gc.ChangeState<SelectUnitState>();
        }
        else
        {
            if (gc.currentCharacter is Player)
            {
                gc.ChangeState<CommandSelectionState>();
            }
            else if (gc.currentCharacter is Enemy)
            {
                gc.ChangeState<EnemyTurnState>();
            }
        }
        yield break;
    }

}