using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : Item
{
    public AudioClip useClip;
    public IntentTypes.Intent itemIntent;

    public virtual void Use(Character _character)
    {
        Use(_character.controller);
    }

    public abstract void Use(CharController _target);
}
