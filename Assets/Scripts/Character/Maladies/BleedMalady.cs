using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedMalady : Malady
{
    private int countdown;
    private const int MaxIterations = 3;
    private Damage Damage = new Damage(DamageTypes.DamageType.Pierce, 20, true);
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Bleed; }
    }

    public override void ApplyMalady(CharController _target)
    {
        if (_target == null)
            return;

        _target.Damage(Damage);
        _target.bc.battleUI.UpdateCurrentStats(_target.bc.CurrentCharacter == _target);
    }

    public override void RefreshMalady()
    {
        countdown = MaxIterations;
    }

    public override void TurnTick(CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        if (countdown <= 0)
            RemoveMalady();
        countdown -= 1;
        ApplyMalady(target);
    }

    public override void Init(CharController _target)
    {
        base.Init(_target);
        countdown = MaxIterations;
        go = Instantiate(Resources.Load("Prefabs/Malady Effects/BleedEffectPlayer")) as GameObject;
        go.transform.parent = _target.gameObject.transform;
    }

    public override void HideMalady()
    {
        base.HideMalady();
        go.SetActive(false);
    }

    public override void ShowMalady()
    {
        base.HideMalady();
        go.SetActive(true);
    }

}
