using UnityEngine;
using System.Collections;

public class ExploreState : BattleState
{

    public override void Enter()
    {
        Debug.Log("explore");
        base.Enter();
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Debug.Log("Explore State Click");
    }

}