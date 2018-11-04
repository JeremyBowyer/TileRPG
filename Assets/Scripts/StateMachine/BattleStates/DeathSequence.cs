using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSequence : BattleState
{

    public override void Enter()
    {
        base.Enter();
        gc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
