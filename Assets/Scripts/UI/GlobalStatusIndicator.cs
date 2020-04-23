using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStatusIndicator : StatusIndicator
{
    public override void Start()
    {
        base.Start();
        conditionPrefab = Resources.Load("Prefabs/UI/Battle/GlobalCondition") as GameObject;
        maladyChargePrefab = Resources.Load("Prefabs/UI/Battle/GlobalMaladyCharged") as GameObject;
    }
}
