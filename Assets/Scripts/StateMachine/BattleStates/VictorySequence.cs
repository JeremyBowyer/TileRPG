using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySequence : BattleState
{ 
    
    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.grid.ClearGrid();
        gc.protag.transform.position = gc.protagStartPos;
        inTransition = false;
        gc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
