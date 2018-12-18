using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySequence : BattleState
{

    public override bool isInterruptable
    {
        get { return false; }
    }
    public override bool isMaster
    {
        get { return true; }
    }

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
        inTransition = true;
        base.Enter();

        // Hide party members
        Protagonist protag = gc.protag.character as Protagonist;
        foreach (PartyMember member in protag.partyMembers)
        {
            member.controller.gameObject.transform.localScale = Vector3.zero;
        }

        // Deconstruct battle grid
        gc.grid.ClearGrid();

        // Place protag back at starting location
        gc.protag.transform.position = gc.protagStartPos;

        // Clear list of battle character
        gc.battleCharacters.Clear();

        inTransition = false;
        gc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
