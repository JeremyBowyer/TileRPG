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
        Movement m = owner.currentCharacter.GetComponent<TeleportMovement>();
        yield return StartCoroutine(m.Traverse(owner.currentTile));
        if (m.nextTurn)
        {
            m.nextTurn = false;
            owner.ChangeState<SelectUnitState>();
        }
        else                         
        {
            owner.ChangeState<CommandSelectionState>();
        }
    }

}