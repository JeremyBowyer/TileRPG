using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypes
{
    public enum DamageType { None, Fire, Voltaic, Cold, Corruption, Pierce, Bludgeon, Physical };

    public static Color GetColor(DamageType type)
    {
        switch (type)
        {
            case DamageType.Fire:
                return CustomColors.Fire;
            case DamageType.Voltaic:
                return CustomColors.Shock;
            case DamageType.Cold:
                return CustomColors.Frost;
            case DamageType.Corruption:
                return CustomColors.Corruption;
            case DamageType.Pierce:
                return CustomColors.Pierce;
            case DamageType.Bludgeon:
                return CustomColors.Bludgeon;
            case DamageType.Physical:
                return CustomColors.Physical;
            default:
                return CustomColors.White;
        }
    }

    public static GameObject GetEffect(DamageType type)
    {
        switch (type)
        {
            case DamageType.Fire:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/FireDamage");
            case DamageType.Voltaic:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/VolataicDamage");
            case DamageType.Cold:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/ColdDamage");
            case DamageType.Corruption:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/CorruptionDamage");
            case DamageType.Pierce:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/PierceDamage");
            case DamageType.Bludgeon:
                return Resources.Load<GameObject>("Prefabs/Damage Effects/BludgeonDamage");
            case DamageType.Physical:
                return null;
            default:
                return null;
        }
    }

    public static DamageType GetType(MaladyTypes.MaladyType _maladyType)
    {
        switch (_maladyType)
        {
            case MaladyTypes.MaladyType.Burn:
                return DamageType.Fire;
            case MaladyTypes.MaladyType.Shock:
                return DamageType.Voltaic;
            case MaladyTypes.MaladyType.Frost:
                return DamageType.Cold;
            case MaladyTypes.MaladyType.Rot:
                return DamageType.Corruption;
            case MaladyTypes.MaladyType.Bleed:
                return DamageType.Pierce;
            case MaladyTypes.MaladyType.Concussion:
                return DamageType.Bludgeon;
            default:
                return DamageType.None;
        }
    }

    public static IEnumerable GetTypes()
    {
        foreach (DamageType dType in Enum.GetValues(typeof(DamageType)))
        {
            yield return dType;
        }
    }

}
