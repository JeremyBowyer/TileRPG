using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockMalady : Malady
{
    private int countdown;
    private const int MaxIterations = 1;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Shock; }
    }

    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;
        target = _target;
    }

    public override void RefreshMalady()
    {
        countdown = MaxIterations;
    }

    public override void TurnTick(CharController previousCharacter, CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        if (countdown <= 0)
            RemoveMalady();
        countdown -= 1;
    }

    public override void Init(Character _source, CharController _target)
    {
        base.Init(_source, _target);
        countdown = MaxIterations;
        mName = "a shock";
        go = Instantiate(Resources.Load("Prefabs/Malady Effects/ShockEffectPlayer")) as GameObject;
        PSMeshRendererUpdater psUpdater = go.GetComponent<PSMeshRendererUpdater>();
        go.transform.parent = _target.gameObject.transform;
        psUpdater.UpdateMeshEffect(_target.gameObject);
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
