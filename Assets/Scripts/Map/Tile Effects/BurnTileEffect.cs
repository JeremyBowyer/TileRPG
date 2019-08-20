using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnTileEffect : TileEffect
{
    private const int MaxIterations = 2;
    private int countdown;
    private GameObject go;
    private GameObject prefab;
    private Damage dmg = new Damage(DamageTypes.DamageType.Fire, 20, MaladyTypes.MaladyType.Burn, 50);

    public override void ApplyEffect(CharController _target)
    {
        if (_target == null)
            return;

        _target.Damage(dmg);
    }

    public override void TurnTick(CharController _currentCharacter)
    {
    }

    public override void RoundTick()
    {
        ApplyEffect(tile.occupant);
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
    }

    public override void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        base.Init(_tile, _sourceDirection, _grid);

        go = Instantiate(Resources.Load("Prefabs/Tile Effects/BurningEffectTile")) as GameObject;
        PSMeshRendererUpdater psUpdater = go.GetComponent<PSMeshRendererUpdater>();
        go.transform.position = _tile.anchorPoint;
        go.transform.parent = _tile.gameObject.transform;
        psUpdater.UpdateMeshEffect(_tile.gameObject);

        countdown = MaxIterations;
    }

    public override void RemoveEffect()
    {
        Destroy(go);
        base.RemoveEffect();
    }

    public override void ApplyToOccupant()
    {
        CharController occupant = tile.Occupant;
        if (occupant != null)
            ApplyEffect(occupant);
    }
}
