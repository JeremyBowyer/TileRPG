using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEnvironmentSplashTargetState : BattleState
{
    public List<Node> spellRange;
    public List<Node> splashZone;
    public EnvironmentSplashSpellAbility spellAbility;
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
        InTransition = true;
        spellAbility = args.spell as EnvironmentSplashSpellAbility;
        character = bc.CurrentCharacter;
        spellRange = spellAbility.GetRange();

        Color color = AbilityTypes.GetIntentColor(spellAbility.abilityIntent);
        grid.SelectNodes(spellRange, CustomColors.ChangeAlpha(color, 0.04f), "spellrange", "inner");
        grid.OutlineNodes(spellRange, color);
        base.Enter();
        InTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        bc.lineRenderer.positionCount = 0;
        grid.DeSelectNodes("spellrange");
        grid.RemoveOutline(spellRange);
        grid.DeSelectNodes("splashzone");
        spellRange = null;
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
            // If spell isn't a projectile, or the projectile is validated, highlight splashzone because spell is valid
            if (!spellAbility.isProjectile || bc.pvc.ValidateProjectile(spellAbility.GetPath(tile.WorldPosition), tile.gameObject, CustomColors.Hostile, true))
            {
                splashZone = spellAbility.GetSplashZone(tile);
                grid.SelectNodes(splashZone, CustomColors.Hostile, "splashzone", "inner");
            }
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;
        bc.lineRenderer.positionCount = 0;
        grid.DeSelectNodes("splashzone");
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Tile tile = e.info.collider.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            // If spell is a projectile and path isn't valid, return
            if (spellAbility.isProjectile && !bc.pvc.ValidateProjectile(spellAbility.GetPath(tile.WorldPosition), tile.gameObject, CustomColors.Hostile, false))
            {
                return;
            }

                StateArgs spellArgs = new StateArgs
            {
                targetTile = tile,
                spell = spellAbility,
                affectedArea = splashZone,
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

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
        cameraRig.ScreenEdgeMovement(e.info.x, e.info.y);
    }


}
