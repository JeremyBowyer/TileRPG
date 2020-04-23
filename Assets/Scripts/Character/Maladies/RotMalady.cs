using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotMalady : Malady
{
    private const int MAX_ITERATIONS = 2;

    public override MaladyTypes.MaladyType Type
    {
        get { return MaladyTypes.MaladyType.Rot; }
    }


    public override void ApplyMalady(CharController _target, bool queue = false)
    {
        if (_target == null)
            return;

        _target.ChangeMaxHP(-Mathf.RoundToInt(_target.Stats.maxHP * 0.02f));
        _target.ChangeMaxAP(-Mathf.RoundToInt(_target.Stats.maxAP * 0.02f));
        _target.ChangeMaxMP(-Mathf.RoundToInt(_target.Stats.maxMP * 0.02f));
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
        roundTicks = MAX_ITERATIONS;
        go = Instantiate(Resources.Load("Prefabs/Malady Effects/RotEffectPlayer")) as GameObject;
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
