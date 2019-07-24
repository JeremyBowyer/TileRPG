using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIBattleState : BattleState
{
    public Text aiAction;
    protected CharController character;
    protected BaseAI enemyAI;

    protected override void Awake()
    {
        base.Awake();
        aiAction = bc.battleUI.aiAction;
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
        character.ChangeState<EnemyTurnState>();
    }
}
