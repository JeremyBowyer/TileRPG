using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : StateMachine {

    private bool delegatesAssigned;

    public Tile tile;
    public float height
    {
        get
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
                return box.bounds.extents.y;
            else
                return 0f;
        }
        set { }
    }

    public bool outline = false;
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
    public KeepRoom room;
    public SkinnedMeshRenderer[] meshes;
    public Highlight highlight;

    public delegate void OnAttackLand(string _clipName);
    public OnAttackLand onAttackLand;

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

    public List<PartyPassive> PartyPassives
    {
        get { return character.partyPassives; }
    }

    public List<PersonalPassive> PersonalPassives
    {
        get { return character.personalPassives; }
    }

    public List<Party> Parties
    {
        get { return character.parties; }
    }

    public List<Roster> Rosters
    {
        get { return character.rosters; }
    }

    public List<Malady> Maladies
    {
        get { return character.maladies; }
    }

    public List<Boon> Boons
    {
        get { return character.boons; }
    }

    public List<SpellAbility> Spells
    {
        get { return character.abilities; }
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

    public CharacterDamageBonuses DamageBonuses
    {
        get { return character.damageBonuses; }
    }

    public CharacterMaladyBuildUps BuildUps
    {
        get { return character.buildUps; }
    }

    public bool IsDead
    {
        get { return Stats.curHP <= 0; }
    }

    public virtual void InitController()
    {
        AssignReferences();
        SetAnimatorParameters();
        CreateCharacter();
        LoadAudioProfile();
        AssignDelegates();
    }

    public void AssignReferences()
    {
        GameObject bcGO = GameObject.Find("BattleController");
        if (bcGO != null)
            bc = bcGO.GetComponent<BattleController>();

        GameObject lcGO = GameObject.Find("LevelController");
        if (lcGO != null)
            lc = lcGO.GetComponent<LevelController>();

        highlight = GetComponentInChildren<Highlight>();

        audioController = GetComponentInChildren<CharacterAudioController>();

        statusIndicator = transform.Find("CameraAngleTarget/CharacterStatusIndicator").gameObject.GetComponent<CharacterStatusIndicator>();
        
        Transform modelGO = gameObject.transform.Find("Model");
        if(modelGO != null)
        {
            meshes = new SkinnedMeshRenderer[] { modelGO.GetComponent<SkinnedMeshRenderer>() };

        }
        else
        {
            meshes = GetComponentsInChildren<SkinnedMeshRenderer>(false);
        }

    }

    public void AssignDelegates()
    {
        if (delegatesAssigned)
            return;

        BuildUps.onChargeActive += (type) => statusIndicator.AddMaladyCharge(type, true);
        BuildUps.onChargeActive += ChargeMalady;
        BuildUps.onChargeDeactive += statusIndicator.RemoveMaladyCharge;
        delegatesAssigned = true;
    }

    public bool HasPartyPassive(Type _passive)
    {
        foreach (PartyPassive passive in PartyPassives)
        {
            if (passive.GetType() == _passive)
                return true;
        }
        return false;
    }

    public bool HasPersonalPassive(Type _passive)
    {
        foreach (PersonalPassive passive in PersonalPassives)
        {
            if (passive.GetType() == _passive)
                return true;
        }
        return false;
    }

    public bool HasBoon(Boon _boon)
    {
        foreach (Boon boon in Boons)
        {
            if (boon == _boon)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasBoonType(Type _boonType)
    {
        foreach(Boon boon in Boons)
        {
            if(boon.GetType() == _boonType)
            {
                return true;
            }
        }

        return false;
    }

    public void AddBoon(Boon _boon)
    {
        Boons.Add(_boon);
    }

    public void RemoveBoon(Boon _boon)
    {
        Boons.Remove(_boon);
    }

    public bool HasMalady(MaladyTypes.MaladyType _malady)
    {
        return gameObject.GetComponent(MaladyTypes.GetComponentType(_malady)) != null;
    }

    public void AddMalady(Malady _malady)
    {
        Maladies.Add(_malady);
    }

    public void RemoveMalady(Malady _malady)
    {
        Maladies.Remove(_malady);
        statusIndicator.RemoveMalady(MaladyTypes.GetType(_malady));
    }
    
    public virtual void HideCharacter()
    {
        foreach(SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }
        foreach (Malady malady in Maladies)
        {
            malady.HideMalady();
        }
    }

    public virtual void ShowCharacter()
    {
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }
        foreach (Malady malady in Maladies)
        {
            malady.ShowMalady();
        }
    }

    public virtual void CreateCharacter()
    {
        character = new Character();
        character.controller = this;
        character.Init();
    }

    public void LoadAudioProfile()
    {
        audioController.charController = this;
        audioController.LoadProfile();
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
        //BuildUps.ResetBUs();
        RemoveAllMaladies();
        RemoveAllBoons();
    }

    public void OnTurnEnd()
    {

    }

    public virtual void SetAnimatorParameters()
    {
        animParamController = GetComponent<AnimationParameterController>();
    }

    public virtual void Pause()
    {
        animParamController.Pause();
    }

    public virtual void Resume()
    {
        animParamController.Resume();
    }

    public void Place(Tile _tile, bool occupy = true)
    {
        transform.position = _tile.WorldPosition;
        if(occupy)
            OccupyTile(_tile);
    }

    public void PassThrough(Tile _tile)
    {
        OccupyTile(_tile);
    }

    public void Move(Tile _tile)
    {
        Stats.curAP -= (int)_tile.node.gCost*10;
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
        FillAP(-_ability.ApCost);
    }

    public void CastSpell(SpellAbility spell)
    {
        spell.ApplyCost(this);
        bc.battleUI.UpdateCurrentStats(true);
    }

    public void ChangeMaxHP(int amt)
    {
        Stats.ChangeMaxHP(amt);
        if (statusIndicator != null)
            statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
        UpdateStatusIndicators();
    }

    public void ChangeMaxAP(int amt)
    {
        Stats.ChangeMaxAP(amt);
        if (statusIndicator != null)
            statusIndicator.SetCurrentAP(Stats.curAP, Stats.maxAPTemp, Stats.maxAP);
        UpdateStatusIndicators();
    }

    public void ChangeMaxMP(int amt)
    {
        Stats.ChangeMaxMP(amt);
        if (statusIndicator != null)
            statusIndicator.SetCurrentMP(Stats.curMP, Stats.maxMPTemp, Stats.maxMP);
        UpdateStatusIndicators();
    }

    /// <summary>Apply damage to a target character. All damage done by a player to other players should be run through this, to account for any damage modifiers the character may have.</summary>
    public void DealDamage(Damage _dmg, CharController _target)
    {
        ShowUITemp();

        Damage adjDmg = (Damage)_dmg.Clone();

        // Calculate and apply damage
        adjDmg.damageAmount = character.damageBonuses.CalculateDamage(adjDmg);

        if(adjDmg.maladyType != null)
        {
            MaladyTypes.MaladyType type = (MaladyTypes.MaladyType)adjDmg.maladyType;
            if (BuildUps.IsCharged(type))
            {
                _target.ApplyMalady(type, character);
                BuildUps.SetBU(type, 0f);
            }
            else
            {
                ApplyBuildUp(adjDmg);
            }
        }

        _target.TakeDamage(adjDmg);
    }

    /// <summary>Apply damage package to a target character. All damage done by a player to other players should be run through this, to account for any damage modifiers the character may have.</summary>
    public void DealDamage(Damage[] _dmgPackage, CharController _target)
    {
        if (gameObject == null)
            return;

        foreach (Damage dmg in _dmgPackage)
        {
            DealDamage(dmg, _target);
        }
    }

    /// <summary>Apply damage package to this character.</summary>
    public void TakeDamage(Damage[] _dmgPackage)
    {
        if (gameObject == null)
            return;

        foreach(Damage dmg in _dmgPackage)
        {
            TakeDamage(dmg);
        }
    }

    /// <summary>Apply damage to this character.</summary>
    public void TakeDamage(Damage _dmg)
    {
        ShowUITemp();

        if (gameObject == null)
            return;

        // Create copy of damage, so damage adjustments don't taint the ability's damage object
        Damage adjDmg = (Damage)_dmg.Clone();

        // Adjust damage by resists, then apply
        adjDmg.damageAmount = Resists.CalculateDamage(adjDmg);
        Stats.Damage((int)adjDmg.damageAmount);

        DisplayEffect((DamageTypes.DamageType)adjDmg.damageType);
        statusIndicator.FloatText((int)adjDmg.damageAmount, DamageTypes.GetColor((DamageTypes.DamageType)adjDmg.damageType));
        CameraController.instance.Shake();
        FlashHighlight(CustomColors.White);

        // Update status indicator
        statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
        UpdateStatusIndicators();

        // Check for death
        if (Stats.curHP <= 0)
        {
            Die(adjDmg);
            CombatLogController.instance.AddEntry(adjDmg, character, true);
        }
        else
        {
            animParamController.SetTrigger("get_hit");
            audioController.Play("get_hit");
            CombatLogController.instance.AddEntry(adjDmg, character, false);
        }
    }

    /// <summary> Apply malady build-up from a given damage source.</summary>
    public void ApplyBuildUp(Damage dmg)
    {
        if (dmg.maladyType != null && dmg.maladyAmount != null)
        {
            MaladyTypes.MaladyType type = (MaladyTypes.MaladyType)dmg.maladyType;
            float curBU = BuildUps.GetBU(type);
            float addBU = (float)dmg.maladyAmount;
            BuildUps.AddBU(type, addBU);
            statusIndicator.DisplayBuildUp(type, curBU, BuildUps.GetBU(type));
        }
    }

    public void AddResistance(DamageTypes.DamageType type, float amt)
    {
        Resists.AddResistance(type, amt);
    }

    public void RemoveResistance(DamageTypes.DamageType type, float amt)
    {
        Resists.RemoveResistance(type, amt);
    }

    public float GetResistance(DamageTypes.DamageType type)
    {
        return Resists.GetResistance(type);
    }

    public void AddDamageBoost(DamageTypes.DamageType type, float amt)
    {
        DamageBonuses.AddBonus(type, amt);
    }

    public void RemoveDamageBoost(DamageTypes.DamageType type, float amt)
    {
        DamageBonuses.RemoveBonus(type, amt);
    }

    public float GetDamageBoost(DamageTypes.DamageType type)
    {
        return DamageBonuses.GetBonus(type);
    }

    public void DisplayEffect(DamageTypes.DamageType type)
    {
        GameObject prefab = DamageTypes.GetEffect(type);
        if (prefab == null)
            return;
        GameObject effectGO = Instantiate(prefab, gameObject.transform);
    }

    public void ChargeMalady(MaladyTypes.MaladyType type)
    {
        audioController.Play("ChargeMalady");
    }

    public void ApplyMalady(MaladyTypes.MaladyType type, Character _source)
    {
        audioController.Play("Malady");
        MaladyTypes.ApplyMalady(type, _source, this);
        statusIndicator.AddMalady(type, true);
    }

    public void Heal(int amt, bool show = false)
    {
        if (gameObject == null)
            return;

        Stats.Heal(amt);
        if(show && statusIndicator != null)
        {
            statusIndicator.FloatText(amt, CustomColors.HP);
            statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP);
        }
        UpdateStatusIndicators();
    }

    public void FillAP(int amt, bool show = false)
    {
        Stats.FillAP(amt);
        if(show && statusIndicator != null)
        {
            statusIndicator.FloatText(amt, CustomColors.AP);
        }
        UpdateStatusIndicators();
    }

    public void FillAP(bool show = false)
    {
        if(show && statusIndicator != null)
        {
            statusIndicator.FloatText(Stats.maxAPTemp - Stats.curAP, CustomColors.AP);
        }
        Stats.FillAP();
        UpdateStatusIndicators();
    }

    public void FillMP(int amt, bool show = false)
    {
        Stats.FillMP(amt);
        if(show && statusIndicator != null)
        {
            statusIndicator.FloatText(amt, CustomColors.MP);
        }
        UpdateStatusIndicators();
    }

    public void FillMP(bool show = false)
    {
        Stats.FillMP();
        if(show && statusIndicator != null)
        {
            statusIndicator.FloatText(Stats.maxMPTemp - Stats.curMP, Color.blue);
        }
        UpdateStatusIndicators();
    }

    public void UpdateStatusIndicators()
    {
        if (BattleUIController.instance == null)
            return;
        BattleUIController.instance.UpdateCurrentStats(bc.CurrentCharacter == this);
        BattleUIController.instance.UpdateTargetStats(bc.TargetCharacter == this);
    }

    public void TurnToward()
    {

    }

    public void ShowUITemp(float delay = 3f)
    {
        if(statusIndicator != null)
            statusIndicator.ShowTemp(delay);
    }

    public void ShowUI(bool show)
    {
        if (statusIndicator == null)
            return;

        statusIndicator.Show();
        if(show)
            statusIndicator.SetCurrentHP(Stats.curHP, Stats.maxHPTemp, Stats.maxHP, animate: false);
    }

    public void HideUI(bool wait = true)
    {
        if (statusIndicator != null)
            statusIndicator.Hide(wait);
    }
    
    public void OnUnitChange(CharController previousCharacter, CharController currentCharacter)
    {
        Stats.FillAP();
    }

    public void OnRoundChange()
    {
        BuildUps.DecreaseAll(5f);
    }

    public virtual void Die(Damage _damage)
    {
        if (gameObject == null)
            return;

        Stats.curHP = 0;
        StateArgs deathArgs = new StateArgs()
        {
            damage = _damage,
            waitingStateMachines = new List<StateMachine> { bc },
            finishInterruptedState = false
        };
        ChangeState<DeathSequence>(deathArgs);
    }

    public virtual void AfterDeath(Damage damage)
    {
        Party[] partiesCopy = new Party[Parties.Count];
        Parties.CopyTo(partiesCopy);
        foreach (Party party in partiesCopy)
        {
            party.RemoveMember(character);
        }

        Roster[] rostersCopy = new Roster[Rosters.Count];
        Rosters.CopyTo(rostersCopy);
        foreach (Roster roster in rostersCopy)
        {
            roster.RemoveMember(character);
        }

        RemoveAllMaladies();
        OccupyTile(null);

        bc.onUnitChange -= OnUnitChange;
        bc.OnUnitDeath(this, damage);

        //statusIndicator.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void RemoveAllMaladies()
    {
        Malady[] MaladiesCopy = new Malady[Maladies.Count];
        Maladies.CopyTo(MaladiesCopy);
        foreach (Malady malady in MaladiesCopy)
        {
            malady.RemoveMalady();
        }
    }

    public void RemoveAllBoons()
    {
        Boon[] BoonsCopy = new Boon[Boons.Count];
        Boons.CopyTo(BoonsCopy);
        foreach(Boon boon in BoonsCopy)
        {
            boon.RemoveBoon();
        }
    }

    public void PlayFootstep()
    {
        audioController.PlayRandomFromGroup(Sound.SoundGroup.Run);
    }
    
    public void PlayClip(string _clipName)
    {
    }

    public void AttackLand(string _clipName)
    {
        audioController.Play("attack");
        onAttackLand?.Invoke(_clipName);
    }

    public void Highlight(Color _color)
    {
        if (highlight == null)
            return;

        highlight.HighlightObject(_color);
    }

    public void RemoveHighlight()
    {
        if (highlight == null)
            return;

        highlight.RemoveHighlight();
    }

    public void FlashHighlight(Color _color)
    {
        if (highlight == null)
            return;

        highlight.FlashObject(_color);
    }

    private void OnDrawGizmos()
    {
        Vector3 forwardleft = new Vector3(-0.5f, 0f, 0.5f);

        Gizmos.color = CustomColors.Hostile;
        //Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + direction * 1f + Vector3.up * 0.5f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.forward * 1f + Vector3.up * 0.5f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.left * 1f + Vector3.up * 0.5f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + forwardleft * 1f + Vector3.up * 0.5f);
    }

}
