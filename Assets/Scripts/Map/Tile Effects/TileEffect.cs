using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEffect : MonoBehaviour, IDamageSource
{
    public Tile tile;
    public BattleController bc;
    public Character source;
    public string tName = "a tile";

    public abstract void TurnTick(CharController previousCharacter, CharController currentCharacter);
    public abstract void RoundTick();

    public abstract void OnCharEnter(CharController _target, bool queue = false);
    public abstract void OnCharExit(CharController _target, bool queue = false);

    public abstract void RefreshEffect();

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

    public virtual void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid, Character _source)
    {
        tile = _tile;
        source = _source;
        bc.onUnitChange += TurnTick;
        bc.onRoundChange += RoundTick;
        //ApplyToOccupant();
    }

    public string GetSourceName()
    {
        return tName;
    }

    public Character GetCharacterSource()
    {
        return source;
    }
}
