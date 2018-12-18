using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCharacterTargetState : BattleState
{
    public List<Node> spellRange;
    public List<GameObject> outlinedEnemies = new List<GameObject>();
    public SpellAbility spellAbility;
    public CharController character;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SpellTargetSequenceState),
            typeof(CommandSelectionState),
            typeof(CheckForTurnEndState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        spellAbility = args.spell;
        character = gc.currentCharacter;
        spellRange = pathfinder.FindRange(gc.currentCharacter.tile.node, spellAbility.AbilityRange, spellAbility.diag, true, true, true);
        grid.HighlightNodes(spellRange);
        base.Enter();
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        grid.UnHighlightNodes(spellRange);
        spellRange = null;
        grid.DeSelectNodes();
        ClearOutlines();
    }

    public void ClearOutlines()
    {
        foreach (GameObject enemy in outlinedEnemies)
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
        UserInputController.mouseLayer = spellAbility.mouseLayer;
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        EnemyController target = e.info.gameObject.GetComponent<EnemyController>();

        if (target == null || target.tile == null)
            return;

        if (spellRange.Contains(target.tile.node))
        {
            GameObject go = target.gameObject;
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 5f;

            outlinedEnemies.Add(target.gameObject);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        ClearOutlines();
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        EnemyController target = e.info.collider.gameObject.GetComponent<EnemyController>();

        if (target == null || target.tile == null)
            return;

        if (spellRange.Contains(e.info.collider.GetComponent<EnemyController>().tile.node))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetCharacter = target,
                spell = spellAbility,
                waitingStateMachines = new List<StateMachine> { gc }
            };
            character.ChangeState<SpellTargetSequenceState>(spellArgs);
            gc.ChangeState<CheckForTurnEndState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }
}
