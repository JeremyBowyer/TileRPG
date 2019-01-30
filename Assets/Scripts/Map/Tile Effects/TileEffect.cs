using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEffect : MonoBehaviour
{
    public Tile tile;
    public BattleController bc;

    public abstract void ApplyEffect(CharController _target);
    public abstract void ApplyToOccupant();

    public virtual void RemoveEffect()
    {
        bc.onUnitChange -= Tick;
        Destroy(this);
    }

    public virtual void Awake()
    {
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
    }

    public virtual void Tick(CharController currentCharacter)
    {
        ApplyToOccupant();
    }

    public virtual void Init(Tile _tile)
    {
        tile = _tile;
        bc.onUnitChange += Tick;
        ApplyToOccupant();
    }
}
