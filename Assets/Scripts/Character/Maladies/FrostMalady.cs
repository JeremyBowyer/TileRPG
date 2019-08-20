using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMalady : Malady
{
    private int countdown;
    private const int MaxIterations = 3;
    private Dictionary<DamageTypes.DamageType, float> resistAmts;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Frost; }
    }

    public override void ApplyMalady(CharController _target)
    {
        if (_target == null)
            return;
        target = _target;
        foreach(DamageTypes.DamageType type in resistAmts.Keys)
        {
            resistAmts[type] = target.AddResistance(type, -0.5f);
        }
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
        resistAmts = new Dictionary<DamageTypes.DamageType, float>();
        resistAmts[DamageTypes.DamageType.Fire] = 0f;
        resistAmts[DamageTypes.DamageType.Voltaic] = 0f;
        resistAmts[DamageTypes.DamageType.Cold] = 0f;
        resistAmts[DamageTypes.DamageType.Corruption] = 0f;
        resistAmts[DamageTypes.DamageType.Pierce] = 0f;
        resistAmts[DamageTypes.DamageType.Bludgeon] = 0f;
        resistAmts[DamageTypes.DamageType.Physical] = 0f;

        go = Instantiate(Resources.Load("Prefabs/Malady Effects/FrostEffectPlayer")) as GameObject;
        PSMeshRendererUpdater psUpdater = go.GetComponent<PSMeshRendererUpdater>();
        go.transform.parent = _target.gameObject.transform;
        psUpdater.UpdateMeshEffect(_target.gameObject);
    }

    public override void RemoveMalady()
    {
        base.RemoveMalady();
        foreach (DamageTypes.DamageType type in resistAmts.Keys)
        {
            resistAmts[type] = target.AddResistance(type, -resistAmts[type]);
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
