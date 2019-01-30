using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BaseAI : MonoBehaviour {

    private CharController character;
    private BattleController bc;
    private CharController _target;

    //private List<Node> moveRange;
    private List<Node> attackRange;
    private List<Node> spellRange;

    private TargetSpellAbility maxSpell;

    static System.Random rnd = new System.Random();

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
        character = GetComponent<CharController>();
        bc = character.bc;
        aiAction = bc.battleUiController.transform.Find("AIAction").GetComponent<Text>();
    }

    public void FindRanges()
    {
        // Establish Ranges
        //moveRange = character.movementAbility.GetNodesInRange(character.stats.moveRange, character.movementAbility.diag, false);
        attackRange = bc.pathfinder.FindRange(character.tile.node, character.AttackAbility.AbilityRange, character.AttackAbility.diag, true, true, false);
        // Further spell range
        // TODO: flesh this out so you're not simply using the spell with farthest range
        float maxSpellRange = 0;
        maxSpell = null;
        foreach (SpellAbility spell in character.Spells)
        {
            if (spell.AbilityRange >= maxSpellRange & spell is TargetSpellAbility)
            {
                maxSpellRange = spell.AbilityRange;
                maxSpell = spell as TargetSpellAbility;
            }
        }
        if (maxSpell != null)
        {
            spellRange = bc.pathfinder.FindRange(bc.CurrentCharacter.tile.node, maxSpell.AbilityRange, maxSpell.diag, true, true, false);
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
        if (!spellRange.Contains(_target.tile.node) & maxSpell.ApCost <= character.Stats.curAP)
            return false;
        return true;
    }

    protected virtual void CastSpell(TargetSpellAbility spell)
    {
        aiAction.text = "Casting spell...";
        StateArgs spellArgs = new StateArgs
        {
            targetCharacter = _target,
            spell = spell,
            waitingStateMachines = new List<StateMachine> { bc },
            callback = callback
        };
        character.ChangeState<SpellTargetSequenceState>(spellArgs);
    }

    protected virtual void Attack()
    {
        aiAction.text = "Attacking...";
        StateArgs attackArgs = new StateArgs
        {
            targetCharacter = _target,
            waitingStateMachines = new List<StateMachine> { bc },
            callback = callback,
            attackAbility = character.AttackAbility
        };
        character.ChangeState<AttackSequenceState>(attackArgs);
    }

    protected virtual void Chase()
    {
        aiAction.text = "Chasing...";
        Tile tile = bc.grid.GetNeighbors(_target.tile.node, true, false)[0].tile;
        List<Node> path = bc.pathfinder.FindPath(bc.CurrentCharacter.tile.node, tile.node, character.Stats.moveRange, character.MovementAbility.diag, character.MovementAbility.ignoreOccupant, character.MovementAbility.ignoreUnwalkable, false);
        StateArgs moveArgs = new StateArgs
        {
            path = path,
            callback = callback
        };
        character.ChangeState<MoveSequenceState>(moveArgs);
    }

    protected virtual bool CheckForEnd()
    {
        float curAP = character.Stats.curAP;
        float minCost = Mathf.Min(new float[] { character.AttackAbility.ApCost, maxSpell == null ? curAP+1 : maxSpell.ApCost });
        float distanceFromTarget = bc.pathfinder.GetDistance(character.tile.node, _target.tile.node);
        if ((curAP < minCost & distanceFromTarget < 20) | curAP <= 10)
        {
            return true;
        }
        return false;
    }

    protected virtual void AcquireTarget()
    {
        GameObject closestPlayer = bc.players[0];
        float closestDistance = Vector3.Distance(transform.position, bc.players[0].transform.position);
        foreach (GameObject player in bc.players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestPlayer = player;
                closestDistance = distance;
            }
        }

        _target = closestPlayer.GetComponent<CharController>();

    }

    protected virtual void DecideAction()
    {
        if (CheckForEnd())
        {
            nextAction = AIState.End;
            return;
        }

        if(attackRange.Contains(_target.tile.node) && spellRange.Contains(_target.tile.node))
        {
            List<AIState> stateList = new List<AIState> { AIState.Attack, AIState.Cast }; 
            int r = rnd.Next(stateList.Count);
            nextAction = stateList[r];
            return;
        }

        if (attackRange.Contains(_target.tile.node))
        {
            nextAction = AIState.Attack;
            return;
        }

        if (spellRange.Contains(_target.tile.node))
        {
            nextAction = AIState.Cast;
            return;
        }
        nextAction = AIState.Chase;
    }
}
