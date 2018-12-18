using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackAbility : BaseAbility
{
    public abstract IEnumerator Initiate(CharController target, Action callback);
}
