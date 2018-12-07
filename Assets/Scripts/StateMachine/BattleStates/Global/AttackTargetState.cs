using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState
{
    List<Node> attackRange;
    Character character;
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
        character = gc.currentCharacter;
        attackAbility = args.attackAbility;
        attackRange = pathfinder.FindRange(character.tile.node, attackAbility.AbilityRange, attackAbility.diag, true, true, false);
        grid.HighlightNodes(attackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.UnHighlightNodes(attackRange);
        attackRange = null;

        foreach(GameObject enemy in outlinedEnemies)
        {
            if (enemy == null)
                return;
            Destroy(enemy.GetComponent<Outline>());
        }
        outlinedEnemies = new List<GameObject>();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Character");
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        Enemy enemy = e.info.gameObject.GetComponent<Enemy>();

        if (enemy == null || enemy.tile == null)
            return;

        if (attackRange.Contains(enemy.tile.node))
        {
            GameObject go = enemy.gameObject;
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 5f;

            outlinedEnemies.Add(enemy.gameObject);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Enemy enemy = e.info.gameObject.GetComponent<Enemy>();

        if (enemy == null)
            return;

        Outline ol = enemy.gameObject.GetComponent<Outline>();

        if (ol == null)
            return;

        Destroy(ol);
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {

        Enemy enemy = e.info.gameObject.GetComponent<Enemy>();

        if (enemy == null || enemy.tile == null)
            return;

        if (attackRange.Contains(e.info.GetComponent<Enemy>().tile.node))
        {
            gc.currentCharacter.attackTarget = e.info.GetComponent<Enemy>();
            StateArgs attackArgs = new StateArgs
            {
                targetCharacter = e.info.GetComponent<Enemy>(),
                waitingStateMachines = new List<StateMachine> { gc }
            };
            character.ChangeState<AttackSequenceState>(attackArgs);
            gc.ChangeState<CheckForTurnEndState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }
}
