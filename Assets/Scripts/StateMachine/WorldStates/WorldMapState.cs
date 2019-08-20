using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapState : WorldState
{
    public SuperUIController superUI { get { return lc.superUI; } }
    protected Protagonist protag;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(WorldExploreState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        lc.PauseGame();
        protag = lc.protag.character as Protagonist;
        superUI.ShowMap();
        InTransition = false;
    }
    public override void Exit()
    {
        base.Exit();
        superUI.HideMap();
        lc.ResumeGame();
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        lc.ChangeState<WorldExploreState>();
    }

    protected override void OnKeyDown(object sender, InfoEventArgs<KeyCode> e)
    {
        if (e.info == KeyCode.M)
            lc.ChangeState<WorldExploreState>();
    }
}
