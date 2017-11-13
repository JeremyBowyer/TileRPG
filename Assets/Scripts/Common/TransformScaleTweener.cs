using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformScaleTweener : Vector3Tweener
{
    protected override void OnUpdate(object sender, System.EventArgs e)
    {
        base.OnUpdate(sender, e);
        transform.localScale = currentValue;
    }
}