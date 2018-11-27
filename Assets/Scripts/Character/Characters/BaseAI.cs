using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BaseAI : MonoBehaviour {

    private Character character;
    private GameController gc;
    private Character _target;

    private List<Node> moveRange;
    private List<Node> attackRange;
    private List<Node> spellRange;

    private TargetSpellAbility maxSpell;

    enum AIState { Attack, Cast, Chase, HealSelf, HealAlly, End };

    private Text currentActionText;

    public void Start()
    {
        
    }

    public void Init() {
        character = GetComponent<Character>();
        gc = character.gc;
        currentActionText = gc.uiController.transform.Find("CurrentAction").GetComponent<Text>();
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

    public virtual IEnumerator TakeTurn()
    {
        int cnt = 3;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1f);
            currentActionText.text = "Thinking...";
            cnt--;
        }

        FindRanges();
        AcquireTarget();
        if (CheckForEnd())
            yield break;
        Chase();
        character.stats.curAP = 0;
        yield break;
        /*
        int cnt = 3;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1f);
            currentActionText.text = "Thinking...";
            cnt--;
        }

        if (CheckSpell())
        {
            cnt = 2;
            while (cnt > 0)
            {
                yield return new WaitForSeconds(1f);
                currentActionText.text = "Casting...";
                cnt--;
            }
            CastSpell(maxSpell);
            yield break;
        }
        else if (attackRange.Contains(_target.tile.node) & character.attackAbility.AbilityCost <= character.stats.curAP)
        {
            cnt = 2;
            while (cnt > 0)
            {
                yield return new WaitForSeconds(1f);
                currentActionText.text = "Attacking...";
                cnt--;
            }
            Attack();
            yield break;
        }
        else if (character.stats.curAP >= 10f)
        {
            cnt = 2;
            while (cnt > 0)
            {
                yield return new WaitForSeconds(1f);
                currentActionText.text = "Chasing...";
                cnt--;
            }
            Chase();
            yield break;
        }
        else
        {
            gc.ChangeState<SelectUnitState>();
        }
        */
        
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
        character.gc.ChangeState<SpellEnvironmentSequenceState>();
    }

    protected virtual void Attack()
    {
        character.attackTarget = _target;
        gc.ChangeState<AttackSequenceState>();
    }

    protected virtual void Chase()
    {
        Tile tile = gc.grid.GetNeighbors(_target.tile.node, true, false)[0].tile;
        List<Node> path = gc.pathfinder.FindPath(gc.currentCharacter.tile.node, tile.node, character.stats.moveRange, character.movementAbility.diag, character.movementAbility.ignoreOccupant, character.movementAbility.ignoreUnwalkable, false);
        StateArgs moveArgs = new StateArgs
        {
            path = path,
            character = gc.currentCharacter
        };
        gc.ChangeState<MoveSequenceState>(moveArgs);
    }

    protected virtual bool CheckForEnd()
    {
        float curAP = character.stats.curAP;
        float minCost = Mathf.Min(new float[] { character.attackAbility.AbilityCost, maxSpell == null ? curAP+1 : maxSpell.AbilityCost });
        float distanceFromTarget = gc.pathfinder.GetDistance(character.tile.node, _target.tile.node);
        if ((curAP < minCost & distanceFromTarget < 20) | curAP <= 10)
        {
            gc.ChangeState<SelectUnitState>();
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
}
