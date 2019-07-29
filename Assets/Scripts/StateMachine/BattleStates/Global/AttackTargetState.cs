using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState
{
    List<Node> attackRange;
    CharController character;
    List<GameObject> outlinedEnemies = new List<GameObject>();
    public AttackAbility attackAbility;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(AttackSequenceState),
            typeof(CommandSelectionState),
            typeof(CheckForTurnEndState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        base.Enter();
        character = bc.CurrentCharacter;
        attackAbility = args.attackAbility;
        attackRange = attackAbility.GetRange();
        grid.SelectNodes(attackRange, CustomColors.ChangeAlpha(CustomColors.AttackRange, 0.04f), "attackrange", "inner");
        grid.OutlineNodes(attackRange, CustomColors.AttackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectNodes("attackrange");
        grid.RemoveOutline(attackRange);

        foreach(GameObject go in outlinedEnemies)
        {
            if (go == null)
                return;
            Destroy(go.GetComponent<Highlight>());
        }
        outlinedEnemies = new List<GameObject>();
        attackRange = null;
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Character");
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        EnemyController enemy = e.info.gameObject.GetComponent<EnemyController>();

        if (enemy == null || enemy.tile == null)
            return;

        if (attackRange.Contains(enemy.tile.node) && attackAbility.ValidateTarget(enemy))
        {
            GameObject go = enemy.gameObject;
            Highlight hl = go.AddComponent<Highlight>();
            hl.HighlightObject(CustomColors.Hostile);
            outlinedEnemies.Add(go);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        EnemyController enemy = e.info.gameObject.GetComponent<EnemyController>();

        if (enemy == null)
            return;

        Highlight hl = enemy.gameObject.GetComponent<Highlight>();

        if (hl == null)
            return;

        Destroy(hl);
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {

        EnemyController enemy = e.info.collider.gameObject.GetComponent<EnemyController>();

        if (enemy == null || enemy.tile == null)
            return;

        if (attackRange.Contains(e.info.collider.GetComponent<EnemyController>().tile.node) && attackAbility.ValidateTarget(enemy))
        {
            StateArgs attackArgs = new StateArgs
            {
                targetCharacter = e.info.collider.GetComponent<EnemyController>(),
                waitingStateMachines = new List<StateMachine> { bc },
                attackAbility = attackAbility
            };
            character.ChangeState<AttackSequenceState>(attackArgs);
            bc.ChangeState<CheckForTurnEndState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
        cameraRig.ScreenEdgeMovement(e.info.x, e.info.y);
    }


}
