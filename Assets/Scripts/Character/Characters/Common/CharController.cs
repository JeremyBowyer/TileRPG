using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : StateMachine {

    public Tile tile;
    public float height;
    public List<Malady> maladies;
    public Vector3 direction
    {
        get { return transform.forward; }
    }

    // References
    public Character character;
    public BattleController bc;
    public LevelController lc;
    public CharacterStatusIndicator statusIndicator;
    public AnimationParameterController animParamController;
    public CharacterAudioController audioController;
    public TurnEntry turnEntry;
    public BSPRoom room;
    public SkinnedMeshRenderer mesh;

    // Properties

    // Check for maladies
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

    public CharacterResistances Resists
    {
        get { return character.resists; }
    }

    public CharacterMaladyBuildUps BuildUps
    {
        get { return character.buildUps; }
    }

    public virtual void Awake()
    {
        statusIndicator = transform.Find("CameraAngleTarget/CharacterStatusIndicator").gameObject.GetComponent<CharacterStatusIndicator>();
        SetAnimatorParameters();
        CreateCharacter();
        GameObject bcGO = GameObject.Find("BattleController");
        if (bcGO != null)
            bc = bcGO.GetComponent<BattleController>();

        GameObject lcGO = GameObject.Find("LevelController");
        if (lcGO != null)
            lc = lcGO.GetComponent<LevelController>();

        mesh = gameObject.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

        height = GetComponent<BoxCollider>().bounds.extents.y;
        maladies = new List<Malady>();
        audioController = GetComponentInChildren<CharacterAudioController>();
        ScaleCharacter();
    }

    public bool HasMalady(MaladyTypes.MaladyType _malady)
    {
        return gameObject.GetComponent(MaladyTypes.GetComponentType(_malady)) != null;
    }

    public void AddMalady(Malady _malady)
    {
        maladies.Add(_malady);
    }

    public void RemoveMalady(Malady _malady)
    {
        maladies.Remove(_malady);
    }

    public void ScaleCharacter()
    {
        transform.localScale = transform.localScale / height;
        height = GetComponent<BoxCollider>().bounds.extents.y;
    }

    public virtual void HideCharacter()
    {
        mesh.enabled = false;
        foreach (Malady malady in maladies)
        {
            malady.HideMalady();
        }
    }

    public virtual void ShowCharacter()
    {
        mesh.enabled = true;
        foreach(Malady malady in maladies)
        {
            malady.ShowMalady();
        }
    }

    public virtual void CreateCharacter()
    {
        character = new Character();
        character.controller = this;
        character.Init();
        maladies = new List<Malady>();
    }

    public virtual void LoadCharacter(Character _character)
    {
        character = _character;
    }

    public virtual void InitBattle()
    {
        Stats.initiativeModifier = 1f;
        animParamController.SetBool("idle");
        GameObject circleGO = transform.Find("UnitCircle").gameObject;
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
        GameObject circleGO = transform.Find("UnitCircle").gameObject;
        if (circleGO == null)
            return;

        circleGO.SetActive(false);
        Stats.Refresh();
        RemoveAllMaladies();
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

    public void PassThrough(Tile _tile)
    {
        OccupyTile(_tile);
    }

    public void Move(Tile _tile)
    {
        Stats.curAP -= (int)_tile.node.gCost*20;
        bc.battleUI.UpdateCurrentStats(true);

        OccupyTile(_tile);
    }

    public void OccupyTile(Tile _tile)
    {
        // If already on tile, return
        if (tile == _tile)
            return;

        // Leave current tile
        if (tile != null)
            tile.Occupant = null;

        // Assign new tile
        tile = _tile;

        if(tile != null)
            tile.Occupant = this;
    }

    public void Attack(CharController _target, AttackAbility _ability)
    {
        Stats.curAP -= _ability.ApCost;
        bc.battleUI.UpdateCurrentStats(true);
    }

    public void CastSpell(SpellAbility spell)
    {
        Stats.curMP -= spell.MpCost;
        Stats.curAP -= spell.ApCost;
        bc.battleUI.UpdateCurrentStats(true);
    }

    public void ChangeMaxHP(int amt)
    {
        Stats.ChangeMaxHP(amt);
        statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
    }

    public void ChangeMaxAP(int amt)
    {
        Stats.ChangeMaxAP(amt);
        statusIndicator.SetCurrentAP(Stats.curAP, Stats.maxAPTemp, Stats.maxAP);
    }

    public void ChangeMaxMP(int amt)
    {
        Stats.ChangeMaxMP(amt);
        statusIndicator.SetCurrentMP(Stats.curMP, Stats.maxMPTemp, Stats.maxMP);
    }

    public void Damage(Damage[] dmgPackage)
    {
        if (gameObject == null)
            return;

        foreach(Damage dmg in dmgPackage)
        {
            Damage(dmg);
        }
    }

    public void Damage(Damage dmg)
    {
        if (gameObject == null)
            return;
        animParamController.SetTrigger("get_hit");

        // Calculate and apply damage
        int dmgAmt = Resists.CalculateDamage(dmg);
        Stats.Damage(dmgAmt);
        DisplayEffect((DamageTypes.DamageType)dmg.damageType);
        statusIndicator.FloatText(dmgAmt, DamageTypes.GetColor((DamageTypes.DamageType)dmg.damageType));

        // Apply malady build-up
        if (dmg.maladyType != null && dmg.maladyAmount != null && !HasMalady((MaladyTypes.MaladyType)dmg.maladyType))
        {
            MaladyTypes.MaladyType type = (MaladyTypes.MaladyType)dmg.maladyType;
            float curBU = BuildUps.GetBU(type);
            float addBU = (float)dmg.maladyAmount;
            BuildUps.AddBU(type, addBU);
            statusIndicator.DisplayBuildUp(type, curBU, BuildUps.GetBU(type), () => ApplyMalady(type));
        }

        // Update status indicator
        statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
        bc.battleUI.UpdateCurrentStats(bc.CurrentCharacter == this);

        // Check for death
        if (Stats.curHP <= 0)
        {
            Die();
        }
    }

    public float SetResistance(DamageTypes.DamageType type, float amt)
    {
        float realAmt = Resists.SetResistance(type, amt);
        return realAmt;
    }

    public float AddResistance(DamageTypes.DamageType type, float amt)
    {
        float realAmt = Resists.AddResistance(type, amt);
        return realAmt;
    }

    public float GetResistance(DamageTypes.DamageType type)
    {
        return Resists.GetResistance(type);
    }

    public void DisplayEffect(DamageTypes.DamageType type)
    {
        GameObject prefab = DamageTypes.GetEffect(type);
        if (prefab == null)
            return;
        GameObject effectGO = Instantiate(prefab, gameObject.transform);
    }

    public void ApplyMalady(MaladyTypes.MaladyType type)
    {
        audioController.Play("Malady");
        MaladyTypes.ApplyMalady(type, this);
        BuildUps.SetBU(type, 0f);
    }

    public void Heal(int amt)
    {
        if (gameObject == null)
            return;

        Stats.Heal(amt);
        statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
        statusIndicator.FloatText(amt, Color.green);
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

        //statusIndicator.gameObject.SetActive(false);
        bc.onUnitChange -= OnTurnEnd;

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
        RemoveAllMaladies();
        OccupyTile(null);
        //Destroy(this.gameObject);
    }

    public void RemoveAllMaladies()
    {
        Malady[] MaladiesCopy = new Malady[maladies.Count];
        maladies.CopyTo(MaladiesCopy);
        foreach (Malady malady in MaladiesCopy)
        {
            malady.RemoveMalady();
        }
    }

    public void PlayFootstep()
    {
        audioController.PlayRandomFromGroup(Sound.SoundGroup.Run);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = CustomColors.Hostile;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + direction * 1f + Vector3.up * 0.5f);
    }

}
