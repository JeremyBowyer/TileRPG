using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : IDamageSource
{
    // Info
    public string cName;
    public string cClass;
    public string model;
    public Sprite avatar;
    public int experience { get { return characterLevel.experience; } }
    public int level { get { return characterLevel.level; } }
    public float levelProgress { get { return characterLevel.GetCurrentProgressF(); } }
    protected bool isInitialized;

    // Stats
    public CharacterStats stats = new CharacterStats();
    public CharacterLevel characterLevel = new CharacterLevel();
    public CharacterResistances resists = new CharacterResistances();
    public CharacterDamageBonuses damageBonuses = new CharacterDamageBonuses();
    public CharacterMaladyBuildUps buildUps = new CharacterMaladyBuildUps();

    // Spells, Attack and Movement
    public AttackAbility attackAbility;
    public Movement movementAbility;
    public List<SpellAbility> abilities;
    public List<Malady> maladies;
    public List<Boon> boons;
    public List<PartyPassive> partyPassives;
    public List<PersonalPassive> personalPassives;
    public List<PartyPassive> appliedPartyPassives;
    public List<Party> parties;
    public List<Roster> rosters;

    // References
    public CharController controller;
    public CharacterAudioProfile audioProfile;

    public Character()
    {
        isInitialized = false;
        maladies = new List<Malady>();
        boons = new List<Boon>();
        partyPassives = new List<PartyPassive>();
        personalPassives = new List<PersonalPassive>();
        appliedPartyPassives = new List<PartyPassive>();
        parties = new List<Party>();
        rosters = new List<Roster>();
        attackAbility = new PunchAttackAbility(this);
        movementAbility = new WalkMovement(this);
        abilities = new List<SpellAbility>();
    }

    public virtual void Init()
    {
        if (!isInitialized)
        {
            stats.Init();
            resists.Init();
            damageBonuses.Init();
            buildUps.Init();
            isInitialized = true;
        }
    }

    public string GetSourceName()
    {
        return cName;
    }

    public Character GetCharacterSource()
    {
        return this;
    }
}
