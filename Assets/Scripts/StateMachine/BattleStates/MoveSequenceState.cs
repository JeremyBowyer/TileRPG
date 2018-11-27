using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveSequenceState : BattleState
{
    private Tile startingTile;
    private Tile targetTile;
    private List<Node> path;
    private Character character;

    private IEnumerator traverseCoroutine;

    public override void Enter()
    {
        inTransition = true;
        isInterruptable = false;
        base.Enter();

        character = args.character;
        path = args.path;
        traverseCoroutine = character.movementAbility.Traverse(path, OnCoroutineFinish);
        character.Move(path[path.Count - 1].tile);
        StartCoroutine(traverseCoroutine);
    }

    public void OnCoroutineFinish()
    {
        inTransition = false;
        if (character.NextTurn)
        {
            gc.ChangeState<SelectUnitState>();
            return;
        }
        else
        {
            if (gc.currentCharacter is Player)
            {
                gc.ChangeState<CommandSelectionState>();
                return;
            }
            else if (gc.currentCharacter is Enemy)
            {
                gc.ChangeState<EnemyTurnState>();
                return;
            }
        }
    }

    public override void InterruptTransition()
    {
        StopCoroutine(traverseCoroutine);
        character.Place(targetTile);
        character.animParamController.SetBool("idle", true);
        inTransition = false;
    }

    /*
    IEnumerator Sequence()
    {
        Character character = gc.currentCharacter;
        targetTile = character.targetTile;
        traverseCoroutine = character.movementAbility.Traverse(targetTile);
        while (character.movementAbility.isMoving)
            yield return null;
        while (gc._inTransition)
            yield return null;
        if (character.NextTurn)
        {
            gc.ChangeState<SelectUnitState>();
        }
        else
        {
            if (gc.currentCharacter is Player)
            {
                gc.ChangeState<CommandSelectionState>();
            }
            else if (gc.currentCharacter is Enemy)
            {
                gc.ChangeState<EnemyTurnState>();
            }
        }
        yield break;
    }
    */
}