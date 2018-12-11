using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveSequenceState : BattleState
{
    private Tile targetTile;
    private List<Node> path;
    private CharacterController character;

    private Action callback;

    private IEnumerator traverseCoroutine;

    public override bool isInterruptable
    {
        get { return true; }
    }

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
        character = GetComponent<CharacterController>();
        path = args.path;
        callback = args.callback;
        if(path.Count == 0)
        {
            OnCoroutineFinish();
            return;
        }
        targetTile = path[path.Count - 1].tile;
        traverseCoroutine = character.movementAbility.Traverse(path, OnCoroutineFinish);
        character.Move(targetTile);
        StartCoroutine(traverseCoroutine);
    }

    public void OnCoroutineFinish()
    {
        if (callback != null)
            callback();
        inTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition()
    {
        StopCoroutine(traverseCoroutine);
        character.Place(targetTile);
        character.animParamController.SetBool("idle", true);
        OnCoroutineFinish();
    }
}