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
            typeof(CheckForTurnEndState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        spellAbility = args.spell as EnvironmentPathSpellAbility;
        character = bc.CurrentCharacter;
        spellRange = spellAbility.GetRange();
        grid.SelectNodes(spellRange, CustomColors.SpellRange, "spellrange");
        base.Enter();
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        spellRange = null;
        grid.DeSelectNodes("spellrange");
        grid.DeSelectNodes("startnode");
        grid.DeSelectNodes("abilitypath");
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
            if(startNode == null)
            {
                grid.SelectNodes(tile.node, CustomColors.Support, "startnode");
            }
            else
            {
                abilityPath = spellAbility.GetPath(startNode.tile, tile);
                grid.SelectNodes(abilityPath, CustomColors.Support, "abilitypath");
            }
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
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
                StateArgs spellArgs = new StateArgs
                {
                    targetTile = tile,
                    spell = spellAbility,
                    affectedArea = abilityPath,
                    waitingStateMachines = new List<StateMachine> { bc }
                };
                character.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
                bc.ChangeState<CheckForTurnEndState>();
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
