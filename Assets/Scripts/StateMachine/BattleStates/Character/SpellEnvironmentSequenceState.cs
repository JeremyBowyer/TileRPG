using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellEnvironmentSequenceState : BattleState
{
    public static CharController targetCharacter;
    public static CharController character;
    public static Tile targetTile;
    private List<Node> affectedArea;
    public static EnvironmentSpellAbility spell;
    private State stateToNotify;

    private IEnumerator spellCoroutine;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SelectUnitState),
            typeof(CommandSelectionState),
            typeof(EnemyTurnStateGlobal),
            typeof(VictorySequence),
            typeof(DeathSequence)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        character = GetComponent<CharController>();
        spell = args.spell as EnvironmentSpellAbility;
        targetTile = args.targetTile;
        affectedArea = args.affectedArea;
        base.Enter();

        character.CastSpell(spell);
        spellCoroutine = spell.Initiate(targetTile, affectedArea, OnCoroutineFinish);
        StartCoroutine(spellCoroutine);
    }

    public void OnCoroutineFinish()
    {
        Vector3 sourceDirection;
        if (affectedArea.Count > 1)
            sourceDirection = bc.grid.GetDirection(affectedArea[0], affectedArea[1]);
        else
        {
            Vector3 facingDirection = bc.grid.GetDirection(character.tile.node, affectedArea[0]);
            if(facingDirection == grid.forwardDirection || facingDirection == grid.backwardDirection)
            {
                sourceDirection = grid.leftDirection;
            }
            else
            {
                sourceDirection = grid.forwardDirection;
            }
        }
        for (int i = 0; i < affectedArea.Count; i++)
        {
            spell.ApplyTileEffect(affectedArea[i].tile, sourceDirection, grid);
        }
        InTransition = false;
        character.ChangeState<IdleState>();
    }

    public override void InterruptTransition()
    {
        isInterrupting = true;
        StopCoroutine(spellCoroutine);

        Vector3 sourceDirection;
        if (affectedArea.Count > 1)
            sourceDirection = bc.grid.GetDirection(affectedArea[0], affectedArea[1]);
        else
            sourceDirection = bc.grid.GetDirection(character.tile.node, affectedArea[0]);
        for (int i = 0; i < affectedArea.Count; i++)
        {
            spell.ApplyTileEffect(affectedArea[i].tile, sourceDirection, grid);
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("SpellEnvironmentGO"))
        {
            GameObject.Destroy(go);
        }
        character.animParamController.SetBool("idle", true);
        InTransition = false;
        isInterrupting = false;
        character.ChangeState<IdleState>();
    }
}