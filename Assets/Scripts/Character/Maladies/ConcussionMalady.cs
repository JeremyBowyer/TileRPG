using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcussionMalady : Malady
{
    private const int MAX_ITERATIONS = 999;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Concussion; }
    }

    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;
        _target.ChangeMaxMP(-Mathf.RoundToInt(_target.Stats.maxMPTemp * 0.5f));
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
    }

    public override void Init(Character _source, CharController _target)
    {
        base.Init(_source, _target);
        roundTicks = MAX_ITERATIONS;
        ApplyMalady(_target);
    }

    public override void HideMalady()
    {
        base.HideMalady();
        if(go != null)
            go.SetActive(false);
    }

    public override void ShowMalady()
    {
        base.HideMalady();
        if(go != null)
            go.SetActive(true);
    }

}
