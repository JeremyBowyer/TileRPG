using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentSpellAbility : SpellAbility
{
    public abstract IEnumerator Initiate(Tile tile, List<Node> effectedArea, Action callback);
    public abstract void ApplyTileEffect(Tile tile, Vector3 _sourceDirection, Grid _grid);
}
