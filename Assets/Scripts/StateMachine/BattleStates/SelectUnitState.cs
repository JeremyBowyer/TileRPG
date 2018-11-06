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
        index = (index + 1) % gc.characters.Count;
        gc.ChangePlayer(gc.characters[index].GetComponent<Character>());
        yield return null;
        if (gc.currentCharacter is Player)
        {
            gc.ChangeState<CommandSelectionState>();
        }
        else if(gc.currentCharacter is Enemy)
        {
            gc.ChangeState<EnemyTurnState>();
        }
    }
}