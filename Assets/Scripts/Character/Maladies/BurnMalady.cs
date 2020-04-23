using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnMalady : Malady
{
    private const int MAX_ITERATIONS = 2;
    private Damage Damage;
    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Burn; }
    }

    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;
        if (queue)
            bc.damageQueue.Enqueue(new KeyValuePair<CharController, Damage[]>(_target, new Damage[] { Damage }));
        else
            _target.TakeDamage(Damage);
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
        ApplyMalady(target, true);
        if (roundTicks <= 0)
            RemoveMalady();
        roundTicks -= 1;
    }

    public override void Init(Character _source, CharController _target)
    {
        base.Init(_source, _target);
        Damage = new Damage(this as IDamageSource, DamageTypes.DamageType.Fire, 100, _malady: this);
        roundTicks = MAX_ITERATIONS;
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
