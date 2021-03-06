﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEnvironmentPathTargetState : BattleState
{
    public List<Node> spellRange;
    public List<Node> abilityPath;
    public Node startNode;
    public Node endNode;
    public EnvironmentPathSpellAbility spellAbility;
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
        spellAbility = args.spell as EnvironmentPathSpellAbility;
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
        grid.DeSelectNodes("startnode");
        grid.DeSelectNodes("abilitypath");
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

        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            if(startNode == null)
            {
                grid.SelectNodes(tile.node, CustomColors.Support, "startnode", "inner");
            }
            else
            {
                if (tile.node == startNode)
                {
                    abilityPath = new List<Node>() { tile.node };
                    grid.SelectNodes(abilityPath, CustomColors.Support, "abilitypath", "inner");
                }
                else
                {
                    abilityPath = spellAbility.GetPath(startNode.tile, tile);
                    if (abilityPath.Count == 0)
                        return;
                    grid.SelectNodes(abilityPath, CustomColors.Support, "abilitypath", "inner");
                }
            }
            MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Target);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();

        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;
        if(startNode == null)
        {
            grid.DeSelectNodes("startnode");
        } else
        {
            grid.DeSelectNodes("abilitypath");
        }
        MouseCursorController.instance.ShowCursor(MouseCursorController.CursorType.Default);
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        Tile tile = e.info.collider.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            if(startNode == null)
            {
                startNode = tile.node;
            } else
            {
                if(tile.node == startNode)
                {
                    abilityPath = new List<Node>() { tile.node };
                }
                StateArgs spellArgs = new StateArgs
                {
                    targetTile = tile,
                    spell = spellAbility,
                    affectedArea = abilityPath,
                    waitingStateMachines = new List<StateMachine> { bc }
                };
                character.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
                bc.ChangeState<DishOutDamageState>();
            }
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
