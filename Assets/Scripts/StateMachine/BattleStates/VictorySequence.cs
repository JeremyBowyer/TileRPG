using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySequence : BattleState
{ 
    
    public override void Enter()
    {
        base.Enter();
        gc.grid.ClearGrid();
        StartCoroutine(Init());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public IEnumerator Init()
    {
        while (gc._inTransition)
        {
            yield return null;
        }
        gc.ChangeState<WorldExploreState>();
        yield break;
    }

}
