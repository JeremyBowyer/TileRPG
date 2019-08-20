using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventHandler : MonoBehaviour
{
    private static BattleController bc;

    private void Awake()
    {
        bc = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
    }

    public void UnloadTargetCharacter()
    {
        bc.TargetCharacter = null;
    }

    public void LoadTargetCharacter(CharController target)
    {
        bc.TargetCharacter = target;
    }

}
