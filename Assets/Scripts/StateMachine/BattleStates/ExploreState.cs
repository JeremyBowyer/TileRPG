using UnityEngine;
using System.Collections;

public class ExploreState : BattleState
{

    public override void Enter()
    {
        base.Enter();
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Debug.Log("Explore State Click");
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

}