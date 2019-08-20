using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcussionMalady : Malady
{
    private int countdown;
    private const int MaxIterations = 999;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Concussion; }
    }

    public override void ApplyMalady(CharController _target)
    {
        if (_target == null)
            return;
        _target.ChangeMaxMP(-Mathf.RoundToInt(_target.Stats.maxMPTemp * 0.5f));
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
    }

    public override void Init(CharController _target)
    {
        base.Init(_target);
        countdown = MaxIterations;
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
