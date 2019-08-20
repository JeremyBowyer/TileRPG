using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DeathSequence : BattleState
{
    private Tile targetTile;
    private List<Node> path;
    private CharController character;
    private State stateToNotify;
    //public override bool IsMaster
    //{
    //    get { return true; }
    //}
    private IEnumerator traverseCoroutine;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(CommandSelectionState),
            typeof(EnemyTurnStateGlobal)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        //character = args.character;
        character = GetComponent<CharController>();
        character.animParamController.SetTrigger("die", OnAnimationFinish);
    }

    public void OnAnimationFinish()
    {
        character.AfterDeath();
        InTransition = false;
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        OnAnimationFinish();
    }
}