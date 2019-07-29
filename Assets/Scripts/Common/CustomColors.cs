using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomColors
{
    public static Color MovementPath
    {
        get
        {
            return new Color(0f, 0.5f, 1f, 1f);
        }
    }

    public static Color MovementRange
    {
        get
        {
            return new Color(0f, 1f, 1f, 1f);
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
            return new Color(1f, 0f, 0f, 1f);
        }
    }

    public static Color Hostile
    {
        get
        {
            return new Color(1f, 0f, 0f);
        }
    }

    public static Color Heal
    {
        get
        {
            return new Color(0f, 1f, 0f);
        }
    }

    public static Color Support
    {
        get
        {
            return new Color(0f, 0.5f, 0.5f);
        }
    }

    public static Color Fire
    {
        get
        {
            return new Color(1f, 0.25f, 0f, 0.5f);
        }
    }

    public static Color PlayerUI
    {
        get
        {
            return new Color(0.4f, 0.8f, 0.4f, 0.5f);
        }
    }

    public static Color EnemyUI
    {
        get
        {
            return new Color(0.8f, 0.4f, 0.4f, 0.5f);
        }
    }

    public static Color ChangeAlpha(Color _color, float _alpha)
    {
        return new Color(_color.r, _color.g, _color.b, _alpha);
    }

}
