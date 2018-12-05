using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BaseAI : MonoBehaviour {

    private Character character;
    private GameController gc;
    private Character _target;

    private List<Node> moveRange;
    private List<Node> attackRange;
    private List<Node> spellRange;

    private TargetSpellAbility maxSpell;

    private Action callback;

    private AIState nextAction;
    enum AIState { Attack, Cast, Chase, HealSelf, HealAlly, End };

    public Text aiAction;

    public bool EndOfTurn
    {
        get { return CheckForEnd(); }
    }

    public void Start()
    {
        
    }

    public void Init() {
        character = GetComponent<Character>();
        gc = character.gc;
        aiAction = gc.uiController.transform.Find("AIAction").GetComponent<Text>();
    }

    public void FindRanges()
    {
        // Establish Ranges
        moveRange = character.movementAbility.GetNodesInRange(character.stats.moveRange, character.movementAbility.diag, false);
        attackRange = gc.pathfinder.FindRange(character.tile.node, character.attackAbility.AbilityRange, character.attackAbility.diag, true, true, false);
        // Further spell range
        // TODO: flesh this out so you're not simply using the spell with farthest range
        float maxSpellRange = 0;
        maxSpell = null;
        foreach (SpellAbility spell in character.spells)
        {
            if (spell.AbilityRange >= maxSpellRange & spell is TargetSpellAbility)
            {
                maxSpellRange = spell.AbilityRange;
                maxSpell = spell as TargetSpellAbility;
            }
        }
        if (maxSpell != null)
        {
            spellRange = gc.pathfinder.FindRange(gc.currentCharacter.tile.node, maxSpell.AbilityRange, maxSpell.diag, true, true, false);
        }
        else
        {
            spellRange = new List<Node>();
        }
    }

    public virtual void ConsiderOptions(Action _callback)
    {
        aiAction.text = "Thinking...";
        callback = _callback;
        FindRanges();
        AcquireTarget();
        DecideAction(); // Add logic here to decide next action
        TakeNextAction();
    }

    public virtual void TakeNextAction()
    {
        switch(nextAction)
        {
            case AIState.Attack:
                Attack();
                break;
            case AIState.Cast:
                CastSpell(maxSpell);
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.End:
                EndTurn();
                break;
            case AIState.HealAlly:
                HealAlly();
                break;
            case AIState.HealSelf:
                HealSelf();
                break;
            default:
                EndTurn();
                break;
        }
    }

    private void EndTurn()
    {
        callback();
    }

    private void HealSelf()
    {

    }

    private void HealAlly()
    {

    }

    protected virtual bool CheckSpell()
    {
        if (maxSpell == null | spellRange.Count == 0)
            return false;
        if (!spellRange.Contains(_target.tile.node) & maxSpell.AbilityCost <= character.stats.curAP)
            return false;
        return true;
    }

    protected virtual void CastSpell(TargetSpellAbility spell)
    {
        character.CastSpell(spell);
    }

    protected virtual void Attack()
    {
        character.attackTarget = _target;
        gc.ChangeState<AttackSequenceState>();
    }

    protected virtual void Chase()
    {
        aiAction.text = "Chasing...";
        Tile tile = gc.grid.GetNeighbors(_target.tile.node, true, false)[0].tile;
        List<Node> path = gc.pathfinder.FindPath(gc.currentCharacter.tile.node, tile.node, character.stats.moveRange, character.movementAbility.diag, character.movementAbility.ignoreOccupant, character.movementAbility.ignoreUnwalkable, false);
        StateArgs moveArgs = new StateArgs
        {
            path = path,
            callback = callback
        };
        character.stats.curAP = 0;
        if (path.Count == 0)
        {
            callback();
            return;
        }
        character.ChangeState<MoveSequenceState>(moveArgs);
    }

    protected virtual bool CheckForEnd()
    {
        float curAP = character.stats.curAP;
        float minCost = Mathf.Min(new float[] { character.attackAbility.AbilityCost, maxSpell == null ? curAP+1 : maxSpell.AbilityCost });
        float distanceFromTarget = gc.pathfinder.GetDistance(character.tile.node, _target.tile.node);
        if ((curAP < minCost & distanceFromTarget < 20) | curAP <= 10)
        {
            return true;
        }
        return false;
    }

    protected virtual void AcquireTarget()
    {
        GameObject closestPlayer = gc.players[0];
        float closestDistance = Vector3.Distance(transform.position, gc.players[0].transform.position);
        foreach (GameObject player in gc.players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestPlayer = player;
                closestDistance = distance;
            }
        }

        _target = closestPlayer.GetComponent<Character>();

    }

    protected virtual void DecideAction()
    {
        if (CheckForEnd())
            nextAction = AIState.End;
        nextAction = AIState.Chase;
    }
}
