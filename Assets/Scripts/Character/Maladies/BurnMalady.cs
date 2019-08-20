using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnMalady : Malady
{
    private int countdown;
    private const int MaxIterations = 2;
    private Damage Damage = new Damage(DamageTypes.DamageType.Fire, 20);
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Burn; }
    }

    public override void ApplyMalady(CharController _target)
    {
        if (_target == null)
            return;

        _target.Damage(new Damage[] { Damage });
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
        go = Instantiate(Resources.Load("Prefabs/Malady Effects/BurningEffectPlayer")) as GameObject;
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
