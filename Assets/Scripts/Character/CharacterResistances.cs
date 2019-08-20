using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResistances
{
    private Dictionary<DamageTypes.DamageType, float> resistances;

    public void Init()
    {
        resistances = new Dictionary<DamageTypes.DamageType, float>();
        resistances[DamageTypes.DamageType.Fire] = 0f;
        resistances[DamageTypes.DamageType.Voltaic] = 0f;
        resistances[DamageTypes.DamageType.Cold] = 0f;
        resistances[DamageTypes.DamageType.Corruption] = 0f;
        resistances[DamageTypes.DamageType.Pierce] = 0f;
        resistances[DamageTypes.DamageType.Bludgeon] = 0f;
        resistances[DamageTypes.DamageType.Physical] = 0f;
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

    public float AddResistance(DamageTypes.DamageType type, float addRes)
    {
        float amt;
        if(resistances.ContainsKey(type))
            amt = Mathf.Clamp(resistances[type] + addRes, -1f, 1f);
        else
            amt = Mathf.Clamp(addRes, -1f, 1f);

        resistances[type] = amt;
        return amt;
    }

    public float SetResistance(DamageTypes.DamageType type, float res)
    {
        float amt = Mathf.Clamp(res, -1f, 1f);
        resistances[type] = amt;
        return amt;
    }

    public float GetResistance(DamageTypes.DamageType type)
    {
        return resistances[type];
    }

}
