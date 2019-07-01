using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : StateMachine {

    public Tile tile;
    public float height;
    public List<PlayerEffect> effects;
    public Vector3 direction
    {
        get { return transform.forward; }
    }

    // References
    public Character character;
    public BattleController bc;
    public LevelController lc;
    public StatusIndicator statusIndicator;
    public AnimationParameterController animParamController;
    public TurnEntry turnEntry;

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
        GameObject bcGO = GameObject.Find("BattleController");
        if (bcGO != null)
            bc = bcGO.GetComponent<BattleController>();

        GameObject lcGO = GameObject.Find("LevelController");
        if (lcGO != null)
            lc = lcGO.GetComponent<LevelController>();

        height = GetComponent<BoxCollider>().bounds.extents.y;
        ScaleCharacter();
    }

    public void ScaleCharacter()
    {
        transform.localScale = transform.localScale / height;
        height = GetComponent<BoxCollider>().bounds.extents.y;
    }

    public virtual void CreateCharacter()
    {
        character = new Character();
        character.controller = this;
        character.Init();
        effects = new List<PlayerEffect>();
    }

    public virtual void LoadCharacter(Character _character)
    {
        character = _character;
    }

    public virtual void InitBattle()
    {
        Stats.initiativeModifier = 1f;
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

    public void OnTurnEnd()
    {

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
        transform.position = _tile.WorldPosition;
        OccupyTile(_tile);
    }

    public void Move(Tile _tile)
    {
        Stats.curAP -= (int)_tile.node.gCost*10;
        bc.battleUiController.UpdateStats();

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
        bc.battleUiController.UpdateStats();
    }

    public void CastSpell(SpellAbility spell)
    {
        Stats.curMP -= spell.MpCost;
        Stats.curAP -= spell.ApCost;
        bc.battleUiController.UpdateStats();
    }

    public void Damage(int amt)
    {
        if (gameObject == null)
            return;

        Stats.Damage(amt);
        statusIndicator.SetHealth(Stats.curHealth, Stats.maxHealth);
        statusIndicator.FloatText(amt.ToString(), Color.red);
        bc.battleUiController.UpdateStats();
        if (Stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
    {
        if (gameObject == null)
            return;

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
        if (gameObject == null)
            return;

        // Leave current tile
        if (tile != null)
            tile.Occupant = null;

        statusIndicator.gameObject.SetActive(false);
        bc.onUnitChange -= OnTurnEnd;
        bc.characters.Remove(gameObject);
        bc.rc.roundChars.Remove(this);

        StateArgs deathArgs = new StateArgs()
        {
            waitingStateMachines = new List<StateMachine> { bc }
        };
        ChangeState<DeathSequence>(deathArgs);
    }

    public virtual void AfterDeath()
    {
        bc.OnUnitDeath(this);
        gameObject.SetActive(false);
        //Destroy(this.gameObject);
    }

}
