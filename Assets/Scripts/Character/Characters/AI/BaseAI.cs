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
        SortAndValidateSpells();
    }

    public virtual void ConsiderOptions()
    {
        SortAndValidateSpells();
        ClearTargets();
        AcquireTargets();
        FindRanges();
        DecideAction();
    }

    protected virtual void DecideAction()
    {
        if (CheckForEnd())
        {
            character.ChangeState<IdleState>();
            EndTurn();
            return;
        }

        if (maxHealSpell != null && character.Stats.IsDamaged)
        {
            HealSelf();
            return;
        }

        if (maxHealSpell != null && DamagedAllyInRange())
        {
            HealAlly();
            return;
        }

        if (hostileSpellRange.Contains(enemyTarget.tile.node))
        {
            HostileTargetSpell();
            return;
        }

        if (attackRange.Contains(enemyTarget.tile.node) && hostileSpellRange.Contains(enemyTarget.tile.node))
        {
            float r = UnityEngine.Random.value;
            if (r >= 0.5f)
            {
                Attack();
            }
            else
            {
                HostileTargetSpell();
            }
            return;
        }

        if (attackRange.Contains(enemyTarget.tile.node))
        {
            Attack();
            return;
        }

        Chase();
    }

    public void SortAndValidateSpells()
    {
        hostileSpells = new List<SpellAbility>();
        healSpells = new List<SpellAbility>();

        foreach (SpellAbility spell in character.Spells)
        {
            if (spell.abilityIntent == IntentTypes.Intent.Hostile && spell.ValidateCost(character))
            {
                hostileSpells.Add(spell);
            } else if (spell.abilityIntent == IntentTypes.Intent.Heal && spell.ValidateCost(character))
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
        float maxSpellRange = 0f;
        maxHostileSpell = null;
        foreach (SpellAbility spell in hostileSpells)
        {
            if (spell.AbilityRange >= maxSpellRange && spell is TargetSpellAbility)
            {
                maxSpellRange = spell.AbilityRange;
                maxHostileSpell = spell as TargetSpellAbility;
            }
        }
        if (maxHostileSpell != null)
        {
            hostileSpellRange = maxHostileSpell.GetRange();
        }
        else
        {
            hostileSpellRange = new List<Node>();
        }
    }

    private void EndTurn()
    {
        bc.ChangeState<SelectUnitState>();
    }

    private void HealSelf()
    {
        StateArgs args = new StateArgs()
        {
            spell = maxHealSpell,
            waitingStateMachines = new List<StateMachine> { bc }
        };
        character.ChangeState<HealSelfState>(args);
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
            } else if(ally.Stats.curHP < lowestAllyInRange.Stats.curHP)
            {
                lowestAllyInRange = ally;
            }
        }

        if(lowestAllyInRange != null)
        {
            StateArgs args = new StateArgs()
            {
                spell = maxHealSpell,
                targetCharacter = lowestAllyInRange
            };
            character.ChangeState<HealAllyState>(args);
        } else
        {
            EndTurn();
        }

    }

    protected virtual void Attack()
    {
        StateArgs attackArgs = new StateArgs
        {
            targetCharacter = enemyTarget,
            waitingStateMachines = new List<StateMachine> { bc },
            callback = ConsiderOptions,
            attackAbility = character.AttackAbility,
            character = character
        };
        character.ChangeState<AttackState>(attackArgs);
    }

    protected virtual void HostileTargetSpell()
    {
        StateArgs args = new StateArgs()
        {
            spell = maxHostileSpell,
            targetCharacter = enemyTarget
        };
        character.ChangeState<HostileTargetSpellState>(args);
    }

    protected virtual void Chase()
    {
        StateArgs chaseArgs = new StateArgs
        {
            targetCharacter = enemyTarget,
            character = character
        };
        character.ChangeState<ChaseState>(chaseArgs);
    }

    protected virtual bool CheckForEnd()
    {
        float curAP = character.Stats.curAP;
        float minCost = Mathf.Min(new float[] {
            character.AttackAbility.ApCost,
            maxHostileSpell == null ? curAP+1 : maxHostileSpell.ApCost,
            maxHealSpell == null ? curAP+1 : maxHealSpell.ApCost,
            character.MovementAbility.costModifier*10
        });

        if (curAP < minCost)
            return true;
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
            if (allyController.Stats.IsDamaged && allyController.Stats.curHP < allyHp)
            {
                allyTarget = allyController;
                allyHp = allyController.Stats.curHP;
            }
        }

    }

    private bool DamagedAllyInRange()
    {
        foreach (GameObject enemyGO in bc.enemies)
        {
            EnemyController enemyController = enemyGO.GetComponent<EnemyController>();
            if (enemyController.Stats.IsDamaged && healSpellRange.Contains(enemyController.tile.node))
                return true;
        }

        return false;
    }
}
