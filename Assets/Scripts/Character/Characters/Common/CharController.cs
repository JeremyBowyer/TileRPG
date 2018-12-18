using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : StateMachine {

    public Tile tile;
    public float height;

    // References
    public Character character;
    public GameController gc;
    public StatusIndicator statusIndicator;
    public AnimationParameterController animParamController;

    // Properties
    public bool NextTurn
    {
        get { return character.stats.curAP <= 0; }
        set { }
    }

    public string Name
    {
        get { return character.cName; }
    }

    public List<SpellAbility> Spells
    {
        get { return character.spells; }
    }

    public AttackAbility AttackAbility
    {
        get { return character.attackAbility; }
    }

    public Movement MovementAbility
    {
        get { return character.movementAbility; }
    }

    public CharacterStats Stats
    {
        get { return character.stats; }
    }

    public virtual void Awake()
    {
        statusIndicator = transform.Find("CameraAngleTarget/StatusIndicator").gameObject.GetComponent<StatusIndicator>();
        SetAnimatorParameters();
        CreateCharacter();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        height = transform.position.y - transform.Find("GroundChecker").transform.position.y;
    }

    public virtual void CreateCharacter()
    {
        character = new Character();
        character.controller = this;
        character.Init();
    }

    public virtual void InitBattle()
    {
        Stats.Init();
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
        Stats.curAP -= _tile.node.gCost;

        // Leave current tile
        if (tile != null)
            tile.occupant = null;

        // Assign new tile
        tile = _tile;
        tile.occupant = gameObject;
    }

    public void Attack(CharController _target, AttackAbility _ability)
    {
        Stats.curAP -= _ability.AbilityCost;
    }

    public void CastSpell(SpellAbility spell)
    {
        Stats.curAP -= spell.AbilityCost;
    }

    public void Damage(int amt)
    {
        Stats.Damage(amt);
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
        if (Stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
    {
        Stats.Heal(amt);
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
    }

    public void OnTurnEnd(CharController character)
    {
        Stats.FillAP();
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

}
