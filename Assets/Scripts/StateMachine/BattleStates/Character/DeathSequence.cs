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
    private Damage damage;
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
            typeof(CommandSelectionState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        damage = args.damage;
        character = GetComponent<CharController>();
        character.animParamController.SetTrigger("die", OnAnimationFinish);
        //character.animParamController.SetBool("idle", false);
        character.audioController.Play("death");
    }

    public void OnAnimationFinish()
    {
        character.AfterDeath(damage);
        InTransition = false;
    }

    public override void InterruptTransition(bool finish)
    {
        isInterrupting = true;
        OnAnimationFinish();
    }
}