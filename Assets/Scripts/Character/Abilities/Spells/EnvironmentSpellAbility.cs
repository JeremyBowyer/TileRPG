using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentSpellAbility : SpellAbility
{
    public abstract IEnumerator Initiate(Tile tile, Action callback);
}
