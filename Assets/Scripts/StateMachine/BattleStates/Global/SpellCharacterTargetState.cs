using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCharacterTargetState : BattleState
{
    public List<Node> spellRange;
    public List<CharController> highlightedChars = new List<CharController>();
    public TargetSpellAbility spellAbility;
    public CharController character;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SpellTargetSequenceState),
            typeof(CommandSelectionState),
            typeof(UnitTurnState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        spellAbility = (TargetSpellAbility)args.spell;
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
        grid.DeSelectNodes("spellrange");
        grid.RemoveOutline(spellRange);
        bc.lineRenderer.positionCount = 0;
        ClearOutlines();
        RemoveOutlineTargetCharacter();
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
        spellRange = null;
    }

    public void ClearOutlines()
    {
        foreach (CharController character in highlightedChars)
        {
            if (character == null)
                return;
            character.RemoveHighlight();
        }
        highlightedChars = new List<CharController>();

    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = spellAbility.mouseLayer;
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        OutlineTargetCharacter(sender, e);

        CharController target = null;
        Color color = Color.cyan;

        if(spellAbility.abilityIntent == IntentTypes.Intent.Hostile)
        {
            target = e.info.gameObject.GetComponent<EnemyController>();
            color = CustomColors.Hostile;
        } else if(spellAbility.abilityIntent == IntentTypes.Intent.Heal)
        {
            target = e.info.gameObject.GetComponent<PlayerController>();
            color = CustomColors.Heal;
        } else if (spellAbility.abilityIntent == IntentTypes.Intent.Support)
        {
            target = e.info.gameObject.GetComponent<PlayerController>();
            color = CustomColors.Support;
        }

        if (target == null || target.tile == null)
            return;

        // If target is in spell range AND
        // target is valid AND
        // either the spell isn't a projectile, or the projectile path is valid
        if (ValidateTarget(target))
        {
            target.Highlight(color);
            highlightedChars.Add(target);
            MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Target);
            bc.TargetCharacter = target;
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();

        bc.lineRenderer.positionCount = 0;
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
        ClearOutlines();
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        CharController target = null;

        if (spellAbility.abilityIntent == IntentTypes.Intent.Hostile)
        {
            target = e.info.collider.GetComponent<EnemyController>();
        }
        else if (spellAbility.abilityIntent == IntentTypes.Intent.Support || spellAbility.abilityIntent == IntentTypes.Intent.Heal)
        {
            target = e.info.collider.GetComponent<PlayerController>();
        }

        if (target == null || target.tile == null)
            return;

        // If target is in spell range AND
        // target is valid AND
        // either the spell isn't a projectile, or the projectile path is valid
        if (ValidateTarget(target))
        {
            bc.battleUI.UnloadTargetStats();
            ClearOutlines();
            StateArgs spellArgs = new StateArgs
            {
                targetCharacter = target,
                spell = spellAbility,
                waitingStateMachines = new List<StateMachine> { bc }
            };
            character.ChangeState<SpellTargetSequenceState>(spellArgs);
            bc.ChangeState<CommandSelectionState>();
        }
    }

    private bool ValidateTarget(CharController target)
    {
        bool inRange = spellRange.Contains(target.tile.node);
        bool validTarget = spellAbility.ValidateTarget(target);
        bool isProjectile = spellAbility.isProjectile;
        bool validPath = bc.pvc.ValidateProjectile(spellAbility.GetPath(target.tile.WorldPosition), target.tile.gameObject, CustomColors.Hostile, true);

        return inRange && validTarget && (!isProjectile || validPath);
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.battleUI.UnloadTargetStats();
        bc.ChangeState<CommandSelectionState>();
    }

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
        cameraRig.ScreenEdgeMovement(e.info.x, e.info.y);
    }


}
