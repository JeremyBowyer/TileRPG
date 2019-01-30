using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomColors
{
    public static Color MovementPath
    {
        get
        {
            return new Color(0f, 0.5f, 1f, 0.5f);
        }
    }

    public static Color MovementRange
    {
        get
        {
            return new Color(1f, 1f, 1f, 0.2f);
        }
    }
    
    public static Color SpellRange
    {
        get
        {
            return MovementRange;
        }
    }

    public static Color AttackRange
    {
        get
        {
            return MovementRange;
        }
    }

    public static Color Hostile
    {
        get
        {
            return new Color(1f, 0f, 0f);
        }
    }

    public static Color Fire
    {
        get
        {
            return new Color(1f, 0.25f, 0f, 0.5f);
        }
    }

}
