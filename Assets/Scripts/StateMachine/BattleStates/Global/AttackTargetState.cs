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
        //grid.SelectNodes(attackRange, CustomColors.AttackRange, "attackrange", "empty");
        grid.OutlineNodes(attackRange, CustomColors.AttackRange);
    }

    public override void Exit()
    {
        base.Exit();
        //grid.DeSelectNodes("attackrange");
        grid.RemoveOutline(attackRange);

        foreach(GameObject enemy in outlinedEnemies)
        {
            if (enemy == null)
                return;
            Destroy(enemy.GetComponent<Outline>());
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
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 3f;
            
            outlinedEnemies.Add(enemy.gameObject);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        EnemyController enemy = e.info.gameObject.GetComponent<EnemyController>();

        if (enemy == null)
            return;

        Outline ol = enemy.gameObject.GetComponent<Outline>();

        if (ol == null)
            return;

        Destroy(ol);
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
