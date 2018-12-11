using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DeathSequence : BattleState
{
    private Tile targetTile;
    private List<Node> path;
    private CharacterController character;
    private State stateToNotify;

    private IEnumerator traverseCoroutine;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(CommandSelectionState),
            typeof(EnemyTurnState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        //character = args.character;
        character = GetComponent<CharacterController>();
        character.animParamController.SetTrigger("die", OnAnimationFinish);
    }

    public void OnAnimationFinish()
    {
        character.AfterDeath();
        inTransition = false;
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        OnAnimationFinish();
    }
}