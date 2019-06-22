using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPlayerEffect : PlayerEffect
{
    private int countdown;
    private const int MaxIterations = 2;
    private const int Damage = 20;

    public override void ApplyEffect(CharController _target)
    {
        if(_target != null)
            _target.Damage(Damage);
    }

    public override void RefreshEffect()
    {
        countdown = MaxIterations;
    }

    public override void TurnTick(CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
        ApplyEffect(target);
    }

    public override void Init(CharController _target)
    {
        base.Init(_target);
        countdown = MaxIterations;
    }

}
