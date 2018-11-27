using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEnvironmentTargetState : BattleState
{
    public List<Node> spellRange;
    public List<Node> splashZone;
    public SpellAbility spellAbility;

    public override void Enter()
    {
        inTransition = true;
        spellAbility = args.spell;
        base.Enter();
        spellRange = pathfinder.FindRange(gc.currentCharacter.tile.node, spellAbility.AbilityRange, spellAbility.diag, true, true, true);
        grid.HighlightNodes(spellRange);
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        grid.UnHighlightNodes(spellRange);
        spellRange = null;
        grid.DeSelectNodes();
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
            splashZone = gc.pathfinder.FindRange(tile.node, 20, true, true, true, true);
            grid.SelectNodes(splashZone, Color.black);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;
        grid.DeSelectNodes();
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Tile tile = e.info.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        if (spellRange.Contains(tile.node))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetTile = tile,
                spell = spellAbility,
                splashZone = splashZone
            };
            gc.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }
}
