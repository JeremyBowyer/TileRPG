using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIBattleState : BattleState
{
    protected CharController character;
    protected BaseAI enemyAI;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Enter()
    {
        base.Enter();
        character = GetComponent<CharController>();
        enemyAI = GetComponent<BaseAI>();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void InitiateTurn()
    {
        if(character.Stats.curHP <= 0)
            bc.ChangeState<SelectUnitState>();
        else
            character.ChangeState<EnemyTurnState>();
    }
}
