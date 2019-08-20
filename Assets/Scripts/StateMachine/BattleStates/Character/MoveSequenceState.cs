using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveSequenceState : BattleState
{
    private Tile targetTile;
    private List<Node> path;
    private CharController character;

    private Action callback;

    private IEnumerator traverseCoroutine;

    public override bool IsInterruptible
    {
        get { return true; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(DeathSequence),
            typeof(EnemyTurnState),
            typeof(IdleState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        character = GetComponent<CharController>();
        bc.FollowTarget(character.transform);
        path = args.path;
        callback = args.callback;
        if(path.Count == 0)
        {
            OnCoroutineFinish();
            return;
        }
        targetTile = path[path.Count - 1].tile;
        traverseCoroutine = character.MovementAbility.Traverse(path, OnCoroutineFinish);
        StartCoroutine(traverseCoroutine);
    }

    public void OnCoroutineFinish()
    {
        InTransition = false;
        character.Move(targetTile);
        callback?.Invoke();
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