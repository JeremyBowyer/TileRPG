﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMalady : Malady
{
    private const int MAX_ITERATIONS = 3;
    private const float Resist_Amount = -0.5f;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Frost; }
    }

    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;
        target = _target;
        foreach (DamageTypes.DamageType dType in Enum.GetValues(typeof(DamageTypes.DamageType)))
        {
            target.AddResistance(dType, Resist_Amount);
        }
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
        roundTicks = MAX_ITERATIONS;
        go = Instantiate(Resources.Load("Prefabs/Malady Effects/FrostEffectPlayer")) as GameObject;
        PSMeshRendererUpdater psUpdater = go.GetComponent<PSMeshRendererUpdater>();
        go.transform.parent = _target.gameObject.transform;
        psUpdater.UpdateMeshEffect(_target.gameObject);
    }

    public override void RemoveMalady()
    {
        base.RemoveMalady();
        foreach (DamageTypes.DamageType dType in Enum.GetValues(typeof(DamageTypes.DamageType)))
        {
            target.RemoveResistance(dType, Resist_Amount);
        }
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
