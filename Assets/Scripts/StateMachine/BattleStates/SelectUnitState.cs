using UnityEngine;
using System.Collections;

public class SelectUnitState : BattleState
{
    int index = -1;
    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        index = (index + 1) % gc.battleCharacters.Count;
        gc.ChangePlayer(gc.battleCharacters[index].GetComponent<Character>());
        if (gc.currentCharacter is Player)
        {
            inTransition = false;
            gc.ChangeState<CommandSelectionState>();
            return;
        }
        else if (gc.currentCharacter is Enemy)
        {
            inTransition = false;
            gc.ChangeState<EnemyTurnState>();
            return;
        }
        inTransition = false;
    }
}