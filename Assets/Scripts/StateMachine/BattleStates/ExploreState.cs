using UnityEngine;
using System.Collections;

public class ExploreState : BattleState
{
    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Debug.Log("Explore State Click");
    }

}