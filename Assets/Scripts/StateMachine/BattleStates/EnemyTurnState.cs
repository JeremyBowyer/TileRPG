using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EnemyTurnState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine("EnemyAI");
    }
    IEnumerator EnemyAI()
    {
        yield return null;
        gc.ChangeState<SelectUnitState>();
    }
}