using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSequence : BattleState
{

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        inTransition = false;
        gc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
