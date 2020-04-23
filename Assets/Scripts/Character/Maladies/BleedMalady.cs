using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedMalady : Malady
{
    private const int MAX_ITERATIONS = 3;
    private Damage Damage;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Bleed; }
    }

    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;

        _target.TakeDamage(Damage);
        _target.bc.battleUI.UpdateCurrentStats(_target.bc.CurrentCharacter == _target);
    }

    public override void RefreshMalady()
    {
        roundTicks = MAX_ITERATIONS;
    }

    public override void TurnTick(CharController previousCharacter, CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        if (roundTicks <= 0)
            RemoveMalady();
        roundTicks -= 1;
        ApplyMalady(target);
    }

    public override void Init(Character _source, CharController _target)
    {
        base.Init(_source, _target);
        Damage = new Damage(this as IDamageSource, DamageTypes.DamageType.Pierce, 20, true, _malady: this);
        roundTicks = MAX_ITERATIONS;
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
