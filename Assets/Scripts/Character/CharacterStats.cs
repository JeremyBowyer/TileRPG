using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    public int maxHealth = 100;
    public int maxAP = 100;
    public int maxMP = 100;
    public float movementModifier = 0.5f;
    
    public int agility = 100;
    public float initiativeModifier = 1f;

    public float initiative
    {
        get { return agility * initiativeModifier; }
    }

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
        get { return _curAP / 10; }
    }

    public bool IsDamaged
    {
        get { return curHealth < maxHealth; }
    }

    public void Init()
    {
        _curHealth = maxHealth;
        _curAP = maxAP;
        _curMP = maxMP;
        initiativeModifier = 1f;
    }

    public void Damage(int amt)
    {
        curHealth -= amt;
    }

    public void Heal(int amt)
    {
        curHealth += amt;
    }

    public void FillAP(int amt)
    {
        curAP = amt;
    }

    public void FillAP()
    {
        curAP = maxAP;
    }
}

