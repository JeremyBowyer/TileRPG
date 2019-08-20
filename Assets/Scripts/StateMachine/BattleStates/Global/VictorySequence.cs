using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySequence : BattleState
{

    public override bool IsInterruptible
    {
        get { return false; }
    }
    public override bool IsMaster
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
        InTransition = true;
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
        //SceneManager.LoadScene("World");

        bc.TerminateBattle();

        InTransition = false;
        bc.ChangeState<IdleState>();
        bc.lc.ChangeState<WorldExploreState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
