using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState
{
    List<Node> attackRange;
    CharController character;
    List<CharController> outlinedEnemies = new List<CharController>();
    public AttackAbility attackAbility;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(AttackSequenceState),
            typeof(CommandSelectionState),
            typeof(UnitTurnState),
            typeof(DishOutDamageState)
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
        grid.SelectNodes(attackRange, CustomColors.ChangeAlpha(CustomColors.AttackRange, 0.25f), "attackrange", "empty");
        grid.OutlineNodes(attackRange, CustomColors.AttackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectNodes("attackrange");
        grid.RemoveOutline(attackRange);

        ClearOutlines();
        RemoveOutlineTargetCharacter();
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
        attackRange = null;
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Character");
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        OutlineTargetCharacter(sender, e);

        EnemyController enemy = e.info.gameObject.GetComponent<EnemyController>();

        if (enemy == null || enemy.tile == null)
            return;

        if (attackRange.Contains(enemy.tile.node) && attackAbility.ValidateTarget(enemy))
        {
            enemy.Highlight(CustomColors.Hostile);
            outlinedEnemies.Add(enemy);
            MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Target);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();

        EnemyController enemy = e.info.gameObject.GetComponent<EnemyController>();

        if (enemy == null)
            return;

        enemy.RemoveHighlight();
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
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
            bc.ChangeState<DishOutDamageState>();
        }
    }

    public void ClearOutlines()
    {
        foreach (CharController character in outlinedEnemies)
        {
            if (character == null)
                return;
            character.RemoveHighlight();
        }
        outlinedEnemies = new List<CharController>();

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
