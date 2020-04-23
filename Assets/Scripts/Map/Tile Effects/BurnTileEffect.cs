using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnTileEffect : TileEffect
{
    private const int MaxIterations = 2;
    private int countdown;
    private GameObject go;
    private GameObject prefab;
    private Damage Damage;

    public override void OnCharEnter(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;

        if (_target.HasMalady(MaladyTypes.MaladyType.Burn))
            return;

        if (queue)
        {
            bc.damageQueue.Enqueue(new KeyValuePair<CharController, Damage[]>(_target, new Damage[] { Damage }));
        }
        else
        {
            _target.TakeDamage(Damage);
        }
    }

    public override void OnCharExit(CharController _target, bool queue = false)
    {
        throw new System.NotImplementedException();
    }

    public override void TurnTick(CharController previousCharacter, CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        OnCharEnter(tile.occupant, true);
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
    }

    public override void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid, Character _source)
    {
        base.Init(_tile, _sourceDirection, _grid, _source);

        tName = "a burning tile";

        source = _source;
        Damage = new Damage(this as IDamageSource, DamageTypes.DamageType.Fire, 25, MaladyTypes.MaladyType.Burn, 100);
        go = Instantiate(AssetController.GetAsset("burn_tile"), _tile.gameObject.transform) as GameObject;
        go.transform.localPosition = _tile.anchorPointLocal;
        //go.transform.parent = _tile.gameObject.transform;
        countdown = MaxIterations;
    }

    public override void RefreshEffect()
    {
        countdown = MaxIterations;
    }

    public override void RemoveEffect()
    {
        Destroy(go);
        base.RemoveEffect();
    }
}
