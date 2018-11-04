using UnityEngine;
using System.Collections;
public class MoveSequenceState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine("Sequence");
    }

    IEnumerator Sequence()
    {
        Character character = gc.currentCharacter;
        character.Move(gc.currentTile);
        while (character.movementAbility.isMoving)
            yield return null;
        while (gc._inTransition)
            yield return null;
        if (character.movementAbility.nextTurn)
        {
            character.movementAbility.nextTurn = false;
            gc.ChangeState<SelectUnitState>();
        }
        else
        {
            gc.ChangeState<CommandSelectionState>();
        }
        yield break;
    }

}