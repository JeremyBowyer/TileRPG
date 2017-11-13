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
        owner.ChangeState<CommandSelectionState>();
    }

}