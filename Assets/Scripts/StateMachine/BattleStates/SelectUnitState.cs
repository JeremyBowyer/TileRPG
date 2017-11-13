using UnityEngine;
using System.Collections;

public class SelectUnitState : BattleState
{
    int index = -1;
    public override void Enter()
    {
        base.Enter();
        StartCoroutine("ChangeCurrentUnit");
    }
    IEnumerator ChangeCurrentUnit()
    {
        index = (index + 1) % characters.Count;
        turn.Change(characters[index]);
        yield return null;
        owner.ChangeState<CommandSelectionState>();
    }
}