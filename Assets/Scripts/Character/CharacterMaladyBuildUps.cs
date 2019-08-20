using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaladyBuildUps
{
    public Dictionary<MaladyTypes.MaladyType, float> currentBUs;

    public void Init()
    {
        currentBUs = new Dictionary<MaladyTypes.MaladyType, float>();
        currentBUs[MaladyTypes.MaladyType.Burn] = 0f;
        currentBUs[MaladyTypes.MaladyType.Shock] = 0f;
        currentBUs[MaladyTypes.MaladyType.Frost] = 0f;
        currentBUs[MaladyTypes.MaladyType.Rot] = 0f;
        currentBUs[MaladyTypes.MaladyType.Bleed] = 0f;
        currentBUs[MaladyTypes.MaladyType.Concussion] = 0f;
    }

    public void AddBU(MaladyTypes.MaladyType type, float bu)
    {
        if (currentBUs.ContainsKey(type))
            SetBU(type, currentBUs[type] + bu);
        else
            SetBU(type, bu);
    }

    public void SetBU(MaladyTypes.MaladyType type, float bu)
    {
        currentBUs[type] = Mathf.Clamp(bu, 0f, 100f);
    }

    public float GetBU(MaladyTypes.MaladyType type)
    {
        if (currentBUs.ContainsKey(type))
            return currentBUs[type];
        else
            return 0;
    }

}
