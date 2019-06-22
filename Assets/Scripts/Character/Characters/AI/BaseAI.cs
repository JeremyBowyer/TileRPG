using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BaseAI : MonoBehaviour {

    private CharController character;
    private BattleController bc;
    private CharController enemyTarget;
    private CharController allyTarget;

    //private List<Node> moveRange;
    private List<Node> attackRange;
    private List<Node> hostileSpellRange;
    private List<Node> healSpellRange;

    private List<SpellAbility> hostileSpells;
    private List<SpellAbility> validHostileSpells;

    private List<SpellAbility> healSpells;
    private List<SpellAbility> validHealSpells;

    private TargetSpellAbility maxHostileSpell;
    private TargetSpellAbility maxHealSpell;

    static System.Random rnd = new System.Random();

    private Action callback;

    private AIState nextAction;
    enum AIState { Attack, SpellAttack, Chase, HealSelf, HealAlly, End };

    public Text aiAction;

    public bool EndOfTurn
    {
        get { return CheckForEnd(); }
    }

    public void Start()
    {
        
    }

    public void Init()
    {
        character = GetComponent<CharController>();
        bc = character.bc;
        aiAction = bc.battleUiController.transform.Find("AIAction").GetComponent<Text>();

        SortAndValidateSpells();
    }

    public virtual void ConsiderOptions(Action _callback)
    {
        aiAction.text = "Thinking...";
        callback = _callback;
        SortAndValidateSpells();
        ClearTargets();
        AcquireTargets();
        FindRanges();
        DecideAction();
        TakeNextAction();
    }

    public void SortAndValidateSpells()
    {
        hostileSpells = new List<SpellAbility>();
        healSpells = new List<SpellAbility>();
        foreach (SpellAbility spell in character.Spells)
        {
            if (spell.abilityIntent == AbilityTypes.Intent.Hostile && spell.ValidateCost(character))
            {
                hostileSpells.Add(spell);
            } else if (spell.abilityIntent == AbilityTypes.Intent.Heal && spell.ValidateCost(character))
            {
                healSpells.Add(spell);
            }
        }
    }

    public void FindRanges()
    {
        // Establish Ranges
        //moveRange = character.movementAbility.GetNodesInRange(character.stats.moveRange, character.movementAbility.diag, false);
        FindAttackRange();
        FindHostileSpellRange();
        FindHealRange();
    }

    public void FindHealRange()
    {
        // Further spell range
        // TODO: flesh this out so you're not simply using the spell with farthest range
        float maxSpellRange = 0;
        maxHealSpell = null;
        foreach (SpellAbility spell in healSpells)
        {
            if (spell.AbilityRange >= maxSpellRange & spell is TargetSpellAbility)
            {
                maxSpellRange = spell.AbilityRange;
                maxHealSpell = spell as TargetSpellAbility;
            }
        }
        if (maxHealSpell != null)
        {
            healSpellRange = bc.pathfinder.FindRange(
                bc.CurrentCharacter.tile.node,
                maxHealSpell.AbilityRange,
                maxHealSpell.diag,
                true,
                true,
                false,
                true);
        }
        else
        {
            healSpellRange = new List<Node>();
        }
    }

    public void FindAttackRange()
    {
        attackRange = bc.pathfinder.FindRange(
            character.tile.node,
            character.AttackAbility.AbilityRange,
            character.AttackAbility.diag,
            true,
            true,
            false,
            true);
    }

    public void FindHostileSpellRange()
    {
        // Further spell range
        // TODO: flesh this out so you're not simply using the spell with farthest range
        float maxSpellRange = 0;
        maxHostileSpell = null;
        foreach (SpellAbility spell in hostileSpells)
        {
            if (spell.AbilityRange >= maxSpellRange & spell is TargetSpellAbility)
            {
                maxSpellRange = spell.AbilityRange;
                maxHostileSpell = spell as TargetSpellAbility;
            }
        }
        if (maxHostileSpell != null)
        {
            hostileSpellRange = bc.pathfinder.FindRange(
                bc.CurrentCharacter.tile.node,
                maxHostileSpell.AbilityRange,
                maxHostileSpell.diag,
                true,
                true,
                false,
                true);
        }
        else
        {
            hostileSpellRange = new List<Node>();
        }
    }

    public virtual void TakeNextAction()
    {
        switch(nextAction)
        {
            case AIState.Attack:
                Attack(enemyTarget);
                break;
            case AIState.SpellAttack:
                CastSpell(maxHostileSpell, enemyTarget);
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
        CastSpell(maxHealSpell, character);
    }

    private void HealAlly()
    {
        // Gather list of damaged allies in range
        List<EnemyController> damagedAlliesInRange = new List<EnemyController>();
        foreach (GameObject allyGO in bc.enemies)
        {
            EnemyController allyController = allyGO.GetComponent<EnemyController>();
            if(allyController.Stats.IsDamaged && healSpellRange.Contains(allyController.tile.node))
            {
                damagedAlliesInRange.Add(allyController);
            }
        }

        // Determine lowest HP ally
        EnemyController lowestAllyInRange = null;
        foreach (EnemyController ally in damagedAlliesInRange)
        {
            if (lowestAllyInRange == null)
            {
                lowestAllyInRange = ally;
            } else if(ally.Stats.curHealth < lowestAllyInRange.Stats.curHealth)
            {
                lowestAllyInRange = ally;
            }
        }

        if(lowestAllyInRange != null)
        {
            CastSpell(maxHealSpell, lowestAllyInRange);
        } else
        {
            EndTurn();
        }

    }

    protected virtual void CastSpell(TargetSpellAbility spell, CharController _target)
    {
        aiAction.text = "Casting spell...";
        Debug.Log(spell.AbilityName);
        Debug.Log(spell.MpCost);
        Debug.Log(character.Stats.curMP);
        StateArgs spellArgs = new StateArgs
        {
            targetCharacter = _target,
            spell = spell,
            waitingStateMachines = new List<StateMachine> { bc },
            callback = callback
        };
        character.ChangeState<SpellTargetSequenceState>(spellArgs);
    }

    protected virtual void Attack(CharController _target)
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
        Tile tile = bc.grid.GetNeighbors(enemyTarget.tile.node, true, false, false)[0].tile;
        List<Node> path = bc.pathfinder.FindPath(
            bc.CurrentCharacter.tile.node,
            tile.node,
            character.Stats.moveRange,
            character.MovementAbility.diag,
            character.MovementAbility.ignoreOccupant,
            character.MovementAbility.ignoreUnwalkable,
            false,
            false);
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
        float minCost = Mathf.Min(new float[] {
            character.AttackAbility.ApCost,
            maxHostileSpell == null ? curAP+1 : maxHostileSpell.ApCost,
            maxHealSpell == null ? curAP+1 : maxHealSpell.ApCost
        });
        float distanceFromTarget = bc.pathfinder.GetDistance(character.tile.node, enemyTarget.tile.node);
        if ((curAP < minCost & distanceFromTarget < 20) | curAP <= 10)
        {
            return true;
        }
        return false;
    }

    protected virtual void ClearTargets()
    {
        enemyTarget = null;
        allyTarget = null;
    }

    protected virtual void AcquireTargets()
    {

        // Acquire closest enemy target
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
        enemyTarget = closestPlayer.GetComponent<CharController>();

        // Acquire closest damaged ally
        if(bc.enemies.Count <= 1)
        {
            return;
        }

        int allyHp = int.MaxValue;

        foreach (GameObject ally in bc.enemies)
        {
            EnemyController allyController = ally.GetComponent<EnemyController>();
            if (allyController.Stats.curHealth < allyController.Stats.maxHealth && allyController.Stats.curHealth < allyHp)
            {
                allyTarget = allyController;
                allyHp = allyController.Stats.curHealth;
            }
        }

    }

    protected virtual void DecideAction()
    {

        if (CheckForEnd())
        {
            nextAction = AIState.End;
            return;
        }

        if (maxHealSpell != null && character.Stats.IsDamaged)
        {
            nextAction = AIState.HealSelf;
            return;
        }

        if (maxHealSpell != null && DamagedAllyInRange())
        {
            nextAction = AIState.HealAlly;
            return;
        }

        if(attackRange.Contains(enemyTarget.tile.node) && hostileSpellRange.Contains(enemyTarget.tile.node))
        {
            List<AIState> stateList = new List<AIState> { AIState.Attack, AIState.SpellAttack }; 
            int r = rnd.Next(stateList.Count);
            nextAction = stateList[r];
            return;
        }

        if (attackRange.Contains(enemyTarget.tile.node))
        {
            nextAction = AIState.Attack;
            return;
        }

        if (hostileSpellRange.Contains(enemyTarget.tile.node))
        {
            nextAction = AIState.SpellAttack;
            return;
        }
        nextAction = AIState.Chase;
    }

    private bool DamagedAllyInRange()
    {
        foreach (GameObject enemyGO in bc.enemies)
        {
            EnemyController enemyController = enemyGO.GetComponent<EnemyController>();
            if (enemyController.Stats.IsDamaged && healSpellRange.Contains(enemyController.tile.node)) return true;
        }

        return false;
    }
}
