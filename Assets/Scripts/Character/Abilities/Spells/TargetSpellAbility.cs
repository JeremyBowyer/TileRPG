using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSpellAbility : SpellAbility
{
    public abstract IEnumerator Initiate(Character character, Action callback);
}
