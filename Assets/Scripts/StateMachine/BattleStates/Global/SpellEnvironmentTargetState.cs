using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEnvironmentTargetState : BattleState
{
    public List<Node> spellRange;
    public List<Node> splashZone;
    public SpellAbility spellAbility;
    public CharController character;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(CommandSelectionState),
            typeof(SpellEnvironmentSequenceState),
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
        grid.DeSelectNodes("splashzone");
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = spellAbility.mouseLayer;
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            splashZone = bc.pathfinder.FindRange(tile.node, 10, true, true, true, true);
            grid.SelectNodes(splashZone, CustomColors.Hostile, "splashzone");
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;
        grid.DeSelectNodes("splashzone");
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Tile tile = e.info.collider.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetTile = tile,
                spell = spellAbility,
                splashZone = splashZone,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            character.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
            bc.ChangeState<CheckForTurnEndState>();
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<CommandSelectionState>();
    }
}
