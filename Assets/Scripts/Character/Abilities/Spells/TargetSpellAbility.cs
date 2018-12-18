using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSpellAbility : SpellAbility
{
    public abstract IEnumerator Initiate(CharController character, Action callback);
}
