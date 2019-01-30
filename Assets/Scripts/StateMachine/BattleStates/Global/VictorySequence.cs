using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        ProtagonistController protag = bc.protag;
        protag.TerminateBattle();
        foreach (PartyMember member in protag.partyMembers)
        {
            member.controller.gameObject.transform.localScale = Vector3.zero;
            member.controller.TerminateBattle();
        }

        PersistentObjects.SaveProtagonist(bc);
        SceneManager.LoadScene("World");
        /*
        // Deconstruct battle grid
        bc.grid.ClearGrid();

        // Place protag back at starting location
        bc.protag.transform.position = bc.protagStartPos;

        // Clear list of battle character
        bc.characters.Clear();

        bc.TerminateBattle();

        inTransition = false;
        bc.ChangeState<WorldExploreState>();
        */
    }

    public override void Exit()
    {
        base.Exit();
    }

}
