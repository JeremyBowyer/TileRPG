using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Malady : MonoBehaviour
{
    public CharController target;
    BattleController bc;

    public abstract void TurnTick(CharController currentCharacter);
    public abstract void RoundTick();
    public abstract void ApplyMalady(CharController _target);
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

    public virtual void Init(CharController _target)
    {
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

}
