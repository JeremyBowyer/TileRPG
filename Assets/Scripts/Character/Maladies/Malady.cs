using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Malady : MonoBehaviour, IDamageSource
{
    public Character source;
    public string mName = "a malady";
    public CharController target;
    protected BattleController bc;

    public abstract void TurnTick(CharController previousCharacter, CharController currentCharacter);
    public abstract void RoundTick();
    public abstract void ApplyMalady(CharController _target, bool queue = false);
    public abstract void RefreshMalady();
    protected GameObject go;
    public Sprite icon;
    public abstract MaladyTypes.MaladyType Type { get; }

    public virtual void RemoveMalady()
    {
        target.RemoveMalady(this);
        bc.onUnitChange -= TurnTick;
        bc.onRoundChange -= RoundTick;
        if (go != null)
            Destroy(go);
        Destroy(this);
    }

    public virtual void Awake()
    {
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        icon = MaladyTypes.GetIcon(Type);
    }

    public virtual void Init(Character _source, CharController _target)
    {
        source = _source;
        target = _target;
        bc.onUnitChange += TurnTick;
        bc.onRoundChange += RoundTick;
        _target.AddMalady(this);
    }

    public virtual void HideMalady()
    {

    }

    public virtual void ShowMalady()
    {

    }

    public virtual string GetSourceName()
    {
        return mName;
    }

    public virtual Character GetCharacterSource()
    {
        return source;
    }

}
