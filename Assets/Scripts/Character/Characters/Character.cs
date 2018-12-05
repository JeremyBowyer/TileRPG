using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : StateMachine {

    public Tile tile;
    public Tile targetTile;
    public CharacterStats stats = new CharacterStats();
    public AttackAbility attackAbility;
    public Movement movementAbility;
    public AnimationParameterController animParamController;
    public string characterName;
    public List<SpellAbility> spells;
    public Character attackTarget;

    public bool NextTurn
    {
        get { return stats.curAP <= 0; }
        set { }
    }

    // References
    public GameController gc;
    public StatusIndicator statusIndicator;

    public float height;

    [System.Serializable]
    public class CharacterStats
    {
        public int maxHealth = 100;
        public int maxAP = 100;
        public int maxMP = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        private int _curAP;
        public int curAP
        {
            get { return _curAP; }
            set { _curAP = Mathf.Clamp(value, 0, maxAP); }
        }

        private int _curMP;
        public int curMP
        {
            get { return _curMP; }
            set { _curMP = Mathf.Clamp(value, 0, maxMP); }
        }

        public int moveRange
        {
            get { return _curAP / 2; }
        }

        public void Init()
        {
            _curHealth = maxHealth;
            _curAP = maxAP;
            _curMP = maxMP;
        }
    }

    public void Awake()
    {
        SetAnimatorParameters();
        spells = new List<SpellAbility>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        height = transform.position.y - transform.Find("GroundChecker").transform.position.y;
    }

    public virtual void InitBattle()
    {
        animParamController.SetBool("idle");
    }

    public virtual void SetAnimatorParameters()
    {
        animParamController = GetComponent<AnimationParameterController>();
        animParamController._bools = new List<string> { "idle", "falling", "running" };
        animParamController._triggers = new List<string> { "jump", "die", "attack" };
    }

    public void Place(Tile _tile)
    {

        //float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.y + gameObject.GetComponent<BoxCollider>().bounds.extents.y;

        Vector3 _targetPos = _tile.transform.position;
        transform.position = _tile.transform.position + new Vector3(0, height, 0);

        // Leave current tile
        if (tile != null)
            tile.occupant = null;

        // Assign new tile
        tile = _tile;
        tile.occupant = gameObject;
    }

    public void Move (Tile _tile)
    {
        stats.curAP -= _tile.node.gCost;

        // Leave current tile
        if (tile != null)
            tile.occupant = null;

        // Assign new tile
        tile = _tile;
        tile.occupant = gameObject;
    }

    public void Attack(Character _target, AttackAbility _ability)
    {
        stats.curAP -= _ability.AbilityCost;
    }

    public void CastSpell(SpellAbility spell)
    {
        stats.curAP -= spell.AbilityCost;
    }

    public void Damage(int amt)
    {
        stats.curHealth -= amt;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        if (stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void OnTurnEnd(Character character)
    {
        fillAP();
    }

    public virtual void Die()
    {
        statusIndicator.gameObject.SetActive(false);
        gc.onUnitChange -= OnTurnEnd;
        gc.battleCharacters.Remove(gameObject);
        gc.characters.Remove(this.gameObject);
        StateArgs deathArgs = new StateArgs()
        {
            waitingStateMachines = new List<StateMachine> { gc }
        };
        ChangeState<DeathSequence>(deathArgs);
    }

    public virtual void AfterDeath()
    {
        gc.OnUnitDeath(this);
        Destroy(this.gameObject);
    }

    public void fillAP(int amt)
    {
        stats.curAP = amt;
    }

    public void fillAP()
    {
        stats.curAP = stats.maxAP;
    }

}
