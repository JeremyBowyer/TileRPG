using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentSplashSpellAbility : EnvironmentSpellAbility
{
    public abstract List<Node> GetSplashZone(Tile _target);
}
