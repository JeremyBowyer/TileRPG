using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    // Info
    public string cName;
    public string cClass;
    public int experience;
    public int level;
    protected bool isInitialized;

    // Stats
    public CharacterStats stats = new CharacterStats();
    public CharacterResistances resists = new CharacterResistances();
    public CharacterMaladyBuildUps buildUps = new CharacterMaladyBuildUps();

    // Spells, Attack and Movement
    public AttackAbility attackAbility;
    public Movement movementAbility;
    public List<SpellAbility> spells;

    // References
    public CharController controller;

    public Character()
    {
        isInitialized = false;
    }

    public virtual void Init()
    {
        if (!isInitialized)
        {
            stats.Init();
            resists.Init();
            buildUps.Init();
            experience = 0;
            level = 1;
            isInitialized = true;
        }
    }

    public virtual void InitAbilities()
    {
        spells = new List<SpellAbility>();
        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
    }


}
