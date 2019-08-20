using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomColors
{
    public static Color Shock
    {
        get
        {
            return SpellRange;
        }
    }

    public static Color Frost
    {
        get
        {
            return new Color(0.5f, 0.85f, 0.8f);
        }
    }

    public static Color Pierce
    {
        get
        {
            return new Color(0.6f, 0.1f, 0.1f);
        }
    }

    public static Color Bludgeon
    {
        get
        {
            return new Color(0.75f, 0.75f, 0.75f);
        }
    }

    public static Color Corruption
    {
        get
        {
            return new Color(0.3f, 0f, 0.6f);
        }
    }

    public static Color White
    {
        get
        {
            return new Color(1f, 1f, 1f);
        }
    }

    public static Color Blank
    {
        get
        {
            return new Color(0f, 0f, 0f, 0f);
        }
    }

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
            return new Color(1f, 0.5f, 0f, 0.5f);
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
