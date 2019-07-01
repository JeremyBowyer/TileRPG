using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEffect : MonoBehaviour
{
    public Tile tile;
    public BattleController bc;

    public abstract void TurnTick(CharController currentCharacter);
    public abstract void RoundTick();

    public abstract void ApplyEffect(CharController _target);
    public abstract void ApplyToOccupant();

    public virtual void RemoveEffect()
    {
        bc.onUnitChange -= TurnTick;
        bc.onRoundChange -= RoundTick;
        Destroy(this);
    }

    public virtual void Awake()
    {
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
    }

    public virtual void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        tile = _tile;
        bc.onUnitChange += TurnTick;
        bc.onRoundChange += RoundTick;
        ApplyToOccupant();
    }
}
