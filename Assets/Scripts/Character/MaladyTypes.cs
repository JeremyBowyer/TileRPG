using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaladyTypes
{
    public enum MaladyType { Burn, Shock, Frost, Rot, Bleed, Concussion };

    public static Sprite GetIcon(MaladyType type)
    {
        switch (type)
        {
            case MaladyType.Burn:
                return Resources.Load<Sprite>("Sprites/FantasyIcons/Burn");
            case MaladyType.Shock:
                return Resources.Load<Sprite>("Sprites/FantasyIcons/Shock");
            case MaladyType.Frost:
                return Resources.Load<Sprite>("Sprites/FantasyIcons/Frost");
            case MaladyType.Rot:
                return Resources.Load<Sprite>("Sprites/FantasyIcons/Rot");
            case MaladyType.Bleed:
                return Resources.Load<Sprite>("Sprites/FantasyIcons/Bleed");
            case MaladyType.Concussion:
                return Resources.Load<Sprite>("Sprites/Icons/Spells/Square Icons/Unlocked/Interrogation");
            default:
                break;
        }

        return null;
    }

    public static Color GetColor(MaladyType type)
    {
        switch (type)
        {
            case MaladyType.Burn:
                return CustomColors.Fire;
            case MaladyType.Shock:
                return CustomColors.Shock;
            case MaladyType.Frost:
                return CustomColors.Frost;
            case MaladyType.Rot:
                return CustomColors.Corruption;
            case MaladyType.Bleed:
                return CustomColors.Pierce;
            case MaladyType.Concussion:
                return CustomColors.Bludgeon;
            default:
                return CustomColors.Blank;
        }
    }

    public static Type GetComponentType(MaladyType type)
    {
        switch (type)
        {
            case MaladyType.Burn:
                return typeof(BurnMalady);
            case MaladyType.Shock:
                return typeof(ShockMalady);
            case MaladyType.Frost:
                return typeof(FrostMalady);
            case MaladyType.Rot:
                return typeof(RotMalady);
            case MaladyType.Bleed:
                return typeof(BleedMalady);
            case MaladyType.Concussion:
                return typeof(ConcussionMalady);
            default:
                return null;
        }
    }

    public static void ApplyMalady(MaladyType type, CharController character)
    {
        if (character == null)
            return;

        ApplyMalady(GetComponentType(type), character);
    }

    public static void ApplyMalady(Type type, CharController character)
    {
        Malady existingMalady = character.gameObject.GetComponent(type) as Malady;
        if (existingMalady != null)
        {
            existingMalady.RefreshMalady();
        }
        else
        {
            Malady malady = character.gameObject.AddComponent(type) as Malady;
            malady.Init(character);
        }
    }

}
