using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Consumable
{
    private int power;

    public Potion()
    {
        power = 25;
        iName = "Potion";
    }

    public override void Use(CharController _target)
    {
        _target.Stats.Heal(power);
    }
}
