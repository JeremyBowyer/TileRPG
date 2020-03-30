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
            typeof(UnitTurnState),
            typeof(DishOutDamageState)
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

        Color color = IntentTypes.GetIntentColor(spellAbility.abilityIntent);
        grid.SelectNodes(spellRange, CustomColors.ChangeAlpha(color, 0.25f), "spellrange", "empty");
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
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
        spellRange = null;
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = spellAbility.mouseLayer;
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        OutlineTargetCharacter(sender, e);

        Tile tile = events.GetTile(e.info.gameObject);

        if (tile == null)
            return;

        if (ValidateTarget(tile))
        {
            splashZone = spellAbility.GetSplashZone(tile);
            grid.SelectNodes(splashZone, CustomColors.Hostile, "splashzone", "inner");
            MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Target);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();

        Tile tile = events.GetTile(e.info.gameObject);

        if (tile == null)
            return;

        bc.lineRenderer.positionCount = 0;
        grid.DeSelectNodes("splashzone");
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Tile tile = events.GetTile(e.info.collider.gameObject);

        if (tile == null)
            return;

        if (ValidateTarget(tile))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetTile = tile,
                spell = spellAbility,
                affectedArea = splashZone,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            character.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
            bc.ChangeState<DishOutDamageState>();
        }
    }

    private bool ValidateTarget(Tile tile)
    {
        bool inRange = spellRange.Contains(tile.node);
        bool isProjectile = spellAbility.isProjectile;
        bool validPath = bc.pvc.ValidateProjectile(spellAbility.GetPath(tile.WorldPosition), tile.gameObject, CustomColors.Hostile, true);

        return inRange && (!isProjectile || validPath);
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
