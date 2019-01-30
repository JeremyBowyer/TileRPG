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
        character = bc.CurrentCharacter;
        spellRange = pathfinder.FindRange(bc.CurrentCharacter.tile.node, spellAbility.AbilityRange, spellAbility.diag, true, true, true);
        grid.SelectNodes(spellRange, CustomColors.SpellRange, "spellrange");
        base.Enter();
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        spellRange = null;
        grid.DeSelectNodes("spellrange");
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

        CharController target = null;
        Color color = Color.cyan;

        if(spellAbility.abilityIntent == AbilityTypes.Intent.Hostile)
        {
            target = e.info.gameObject.GetComponent<EnemyController>();
            color = Color.red;
        } else if(spellAbility.abilityIntent == AbilityTypes.Intent.Support)
        {
            target = e.info.gameObject.GetComponent<PlayerController>();
            color = Color.green;
        }

        if (target == null || target.tile == null)
            return;

        if (spellRange.Contains(target.tile.node) && spellAbility.ValidateTarget(target))
        {
            GameObject go = target.gameObject;
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = color;
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
        CharController target = null;

        if (spellAbility.abilityIntent == AbilityTypes.Intent.Hostile)
        {
            target = e.info.collider.GetComponent<EnemyController>();
        }
        else if (spellAbility.abilityIntent == AbilityTypes.Intent.Support)
        {
            target = e.info.collider.GetComponent<PlayerController>();
        }

        if (target == null || target.tile == null)
            return;

        if (spellRange.Contains(target.tile.node) && spellAbility.ValidateTarget(target))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetCharacter = target,
                spell = spellAbility,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            character.ChangeState<SpellTargetSequenceState>(spellArgs);
            bc.ChangeState<CheckForTurnEndState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }
}
