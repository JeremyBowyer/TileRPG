using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCharacterTargetState : BattleState
{
    public List<Node> spellRange;
    public List<GameObject> outlinedEnemies = new List<GameObject>();
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
        Character character = e.info.gameObject.GetComponent<Character>();

        if (character == null || character.tile == null)
            return;

        if (spellRange.Contains(character.tile.node))
        {
            GameObject go = character.gameObject;
            go.AddComponent<Outline>();
            Outline outline = go.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 5f;

            outlinedEnemies.Add(character.gameObject);
        }
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        ClearOutlines();
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Character character = e.info.gameObject.GetComponent<Character>();

        if (character == null || character.tile == null)
            return;

        if (spellRange.Contains(e.info.GetComponent<Enemy>().tile.node))
        {
            StateArgs spellArgs = new StateArgs
            {
                targetCharacter = character,
                spell = spellAbility
            };
            gc.ChangeState<SpellEnvironmentSequenceState>(spellArgs);
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }
}
