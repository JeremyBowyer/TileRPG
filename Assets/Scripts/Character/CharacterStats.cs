using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStats
{
    // Hard Max
    public int maxHP = 100;
    public int maxAP = 100;
    public int maxMP = 100;

    // Temp Max
    private List<int> maxHpPressures;
    private int _maxHPTemp;
    public int maxHPTemp
    {
        get { return _maxHPTemp; }
        set { _maxHPTemp = Mathf.Clamp(value, 1, maxHP); }
    }

    private List<int> maxApPressures;
    private int _maxAPTemp;
    public int maxAPTemp
    {
        get { return _maxAPTemp; }
        set { _maxAPTemp = Mathf.Clamp(value, 1, maxAP); }
    }

    private List<int> maxMpPressures;
    private int _maxMPTemp;
    public int maxMPTemp
    {
        get { return _maxMPTemp; }
        set { _maxMPTemp = Mathf.Clamp(value, 1, maxMP); }
    }

    // Current Stats
    private int _curHP;
    public int curHP
    {
        get { return _curHP; }
        set { _curHP = Mathf.Clamp(value, 0, maxHPTemp); }
    }

    private int _curAP;
    public int curAP
    {
        get { return _curAP; }
        set { _curAP = Mathf.Clamp(value, 0, maxAPTemp); }
    }

    private int _curMP;
    public int curMP
    {
        get { return _curMP; }
        set { _curMP = Mathf.Clamp(value, 0, maxMPTemp); }
    }

    public int agility = 100;
    public float initiativeModifier = 1f;
    public float initiative
    {
        get { return agility * initiativeModifier; }
    }

    public int moveRange
    {
        get { return _curAP / 10; }
    }

    public bool IsDamaged
    {
        get { return curHP < maxHPTemp; }
    }

    public void Init()
    {
        _curHP = maxHP;
        _curAP = maxAP;
        _curMP = maxMP;

        maxHpPressures = new List<int>();
        maxApPressures = new List<int>();
        maxMpPressures = new List<int>();

        maxHPTemp = maxHP;
        maxAPTemp = maxAP;
        maxMPTemp = maxMP;
    }

    public void Refresh()
    {
        curAP = maxAPTemp;
    }

    public void Damage(int amt)
    {
        curHP -= amt;
    }

    public void Heal(int amt)
    {
        curHP += amt;
    }

    public void ChangeMaxHP(int amt)
    {
        maxHPTemp = Mathf.Clamp(maxHPTemp + amt, 1, maxHP);
        int newAmt = Mathf.Min(new int[] { curHP, maxHPTemp });
        curHP = newAmt;
    }

    public void ChangeMaxAP(int amt)
    {
        maxAPTemp = Mathf.Clamp(maxAPTemp + amt, 1, maxAP);
        curAP = Mathf.Min(new int[] { curAP, maxAPTemp });
    }

    public void ChangeMaxMP(int amt)
    {
        maxMPTemp = Mathf.Clamp(maxMPTemp + amt, 1, maxMP);
        curMP = Mathf.Min(new int[] { curMP, maxMPTemp });
    }

    public void FillAP(int amt)
    {
        curAP += amt;
    }

    public void FillAP()
    {
        curAP = maxAPTemp;
    }

    public void FillMP(int amt)
    {
        curMP += amt;
    }

    public void FillMP()
    {
        curMP = maxMPTemp;
    }

}

