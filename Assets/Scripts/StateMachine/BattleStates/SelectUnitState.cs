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
        owner.ChangePlayer(characters[index]);
        yield return null;
        if(owner.currentCharacter is Player)
        {
            owner.ChangeState<CommandSelectionState>();
        }
        else if(owner.currentCharacter is Enemy)
        {
            owner.ChangeState<EnemyTurnState>();
        }
    }
}