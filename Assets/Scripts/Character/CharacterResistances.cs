using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResistances
{
    private Dictionary<DamageTypes.DamageType, float> resistances;
    private Dictionary<DamageTypes.DamageType, List<float>> resPressures;
    private Dictionary<DamageTypes.DamageType, float> Default_Resistances;
    private const float MAX_RES = 1f;
    private const float MIN_RES = -1f;
    private const float DEFAULT_RES = 0f;

    public void Init(Dictionary<DamageTypes.DamageType, float> default_resistances = null)
    {
        if (default_resistances != null)
        {
            Default_Resistances = default_resistances;
        }
        else
        {
            Default_Resistances = new Dictionary<DamageTypes.DamageType, float>();
            foreach (DamageTypes.DamageType dType in DamageTypes.GetTypes())
            {
                Default_Resistances[dType] = DEFAULT_RES;
            }
        }

        resPressures = new Dictionary<DamageTypes.DamageType, List<float>>();
        foreach (DamageTypes.DamageType dType in DamageTypes.GetTypes())
        {
            resPressures[dType] = new List<float>();
        }

        resistances = new Dictionary<DamageTypes.DamageType, float>();
        foreach (DamageTypes.DamageType dType in DamageTypes.GetTypes())
        {
            CalculateResistance(dType);
        }
    }

    public int CalculateDamage(Damage dmg)
    {
        if (dmg.TrueDamage)
            return (int)dmg.damageAmount;

        DamageTypes.DamageType type = (DamageTypes.DamageType)dmg.damageType;
        float amt = (float)dmg.damageAmount;
        float res = resistances[type];

        int newAmt = Mathf.RoundToInt(amt - (amt * res));
        return newAmt;
    }

    public void AddResistance(DamageTypes.DamageType type, float addRes)
    {
        resPressures[type].Add(addRes);
        CalculateResistance(type);
    }

    public void RemoveResistance(DamageTypes.DamageType type, float removeRes)
    {
        resPressures[type].Remove(removeRes);
        CalculateResistance(type);
    }

    public float CalculateResistance(DamageTypes.DamageType type)
    {
        float bns;
        if (Default_Resistances.ContainsKey(type))
            bns = Default_Resistances[type];
        else
            bns = DEFAULT_RES;

        foreach (float addRes in resPressures[type])
        {
            bns += addRes;
        }

        float amt;
        if (resistances.ContainsKey(type))
            amt = Mathf.Clamp(resistances[type] + bns, MIN_RES, MAX_RES);
        else
            amt = Mathf.Clamp(bns, MIN_RES, MAX_RES);

        resistances[type] = amt;
        return amt;
    }

    public float GetResistance(DamageTypes.DamageType type)
    {
        return resistances[type];
    }

    public void ResetRES()
    {
        foreach (DamageTypes.DamageType dType in DamageTypes.GetTypes())
        {
            resPressures[dType] = new List<float>();
            CalculateResistance(dType);
        }
    }
}
