using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentPathSpellAbility : EnvironmentSpellAbility
{
    public abstract int AbilityPathLimit
    {
        get; set;
    }
    public abstract List<Node> GetPath(Tile _start, Tile _end);
}
