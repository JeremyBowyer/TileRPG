using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : StateMachine {

    public Tile tile;
    public float height;
    public Vector3 direction
    {
        get { return transform.forward; }
    }

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
        height = GetComponent<BoxCollider>().bounds.extents.y;
    }

    public virtual void CreateCharacter()
    {
        character = new Character();
        character.controller = this;
        character.Init();
    }

    public virtual void InitBattle()
    {
        //Stats.Init();
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
        animParamController.SetBool("idle");
        GameObject circleGO = transform.Find("SelectionCirclePrefab").gameObject;
        if (circleGO == null)
            return;

        circleGO.SetActive(true);
        Projector proj = circleGO.GetComponent<Projector>();
        proj.material = new Material(proj.material);
        if (this is EnemyController)
        {
            proj.material.SetColor("_Color", Color.red);
        }
        else
        {
            proj.material.SetColor("_Color", Color.green);
        }
    }

    public virtual void TerminateBattle()
    {
        GameObject circleGO = transform.Find("SelectionCirclePrefab").gameObject;
        if (circleGO == null)
            return;

        circleGO.SetActive(false);
    }

    public virtual void SetAnimatorParameters()
    {
        animParamController = GetComponent<AnimationParameterController>();
        /*
        animParamController._bools = new List<string> { "idle", "falling", "running" };
        animParamController._triggers = new List<string> { "jump", "die", "attack" };
        */
    }

    public virtual void Pause()
    {
        animParamController.Pause();
    }

    public virtual void Resume()
    {
        animParamController.Resume();
    }

    public void Place(Tile _tile)
    {
        Vector3 _targetPos = _tile.transform.position;
        transform.position = _tile.transform.position;
        OccupyTile(_tile);
    }

    public void Move (Tile _tile)
    {
        Stats.curAP -= _tile.node.gCost;
        gc.battleUiController.UpdateStats();

        OccupyTile(_tile);
    }

    public void OccupyTile(Tile _tile)
    {
        // Leave current tile
        if (tile != null)
            tile.Occupant = null;

        // Assign new tile
        tile = _tile;
        tile.Occupant = this;
    }

    public void Attack(CharController _target, AttackAbility _ability)
    {
        Stats.curAP -= _ability.ApCost;
        gc.battleUiController.UpdateStats();
    }

    public void CastSpell(SpellAbility spell)
    {
        Stats.curMP -= spell.ApCost;
        gc.battleUiController.UpdateStats();
    }

    public void Damage(int amt)
    {
        Stats.Damage(amt);
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
        statusIndicator.FloatText(amt.ToString(), Color.red);
        gc.battleUiController.UpdateStats();
        if (Stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
    {
        Stats.Heal(amt);
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
        statusIndicator.FloatText(amt.ToString(), Color.green);
    }

    public void TurnToward()
    {

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
