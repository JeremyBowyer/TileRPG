using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EnemyTurnState : BattleState
{
    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        BaseAI enemyAI = gc.currentCharacter.GetComponent<BaseAI>();
        StartCoroutine(enemyAI.TakeTurn());
    }

}