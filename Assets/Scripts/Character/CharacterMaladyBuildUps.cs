using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaladyBuildUps
{
    public Dictionary<MaladyTypes.MaladyType, float> currentBUs;

    public delegate void OnChargeActive(MaladyTypes.MaladyType _type);
    public OnChargeActive onChargeActive;

    public delegate void OnChargeDeactive(MaladyTypes.MaladyType _type);
    public OnChargeDeactive onChargeDeactive;

    public void Init()
    {
        ResetBUs();
    }

    public void ResetBUs()
    {
        currentBUs = new Dictionary<MaladyTypes.MaladyType, float>();

        foreach (MaladyTypes.MaladyType type in (MaladyTypes.MaladyType[])Enum.GetValues(typeof(MaladyTypes.MaladyType)))
        {
            SetBU(type, 0f);
        }
    }

    public void AddBU(MaladyTypes.MaladyType type, float bu)
    {
        if (currentBUs.ContainsKey(type))
            SetBU(type, currentBUs[type] + bu);
        else
            SetBU(type, bu);
    }

    /// <summary> Sets the build-up for a given MaladyTypes.MaladyType. When appropriate, OnChargeActivate and OnChargeDeactivate delegates are invoked.</summary>
    public void SetBU(MaladyTypes.MaladyType type, float bu)
    {
        if (!currentBUs.ContainsKey(type))
            currentBUs[type] = 0f;

        float oldBU = currentBUs[type];
        float newBU = Mathf.Clamp(bu, 0f, 100f);

        currentBUs[type] = newBU;

        if (oldBU >= 100f && newBU < 100f)
            onChargeDeactive?.Invoke(type);

        if (oldBU < 100f && newBU >= 100f)
            onChargeActive?.Invoke(type);
    }

    public float GetBU(MaladyTypes.MaladyType type)
    {
        if (currentBUs.ContainsKey(type))
            return currentBUs[type];
        else
            return 0;
    }

    public void DecreaseAll(float bu)
    {
        foreach (MaladyTypes.MaladyType type in (MaladyTypes.MaladyType[])Enum.GetValues(typeof(MaladyTypes.MaladyType)))
        {
            if(!IsCharged(type))
                SetBU(type, currentBUs[type] - bu);
        }
    }

    /// <summary> Indicates whether the given malady type is currently charged.</summary>
    public bool IsCharged(MaladyTypes.MaladyType type)
    {
        return currentBUs[type] >= 100f;
    }

    /// <summary> Returns a list of MaladyTyeps.MaladyType that this character has charged.</summary>
    public List<MaladyTypes.MaladyType> GetCharges()
    {
        List<MaladyTypes.MaladyType> charges = new List<MaladyTypes.MaladyType>();
        foreach(MaladyTypes.MaladyType type in currentBUs.Keys)
        {
            if (IsCharged(type))
                charges.Add(type);
        }

        return charges;
    }

}
