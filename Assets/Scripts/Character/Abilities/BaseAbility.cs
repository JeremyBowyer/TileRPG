﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseAbility {

    private string abilityName;
    private string abilityDescription;
    private int abilityID;
    private int abilityPower;
    private int apCost;
    private int mpCost;
    private int abilityRange;
    public CharController character;
    public bool diag;
    public bool inProgress;
    public bool nextTurn;
    public int mouseLayer;
    public AbilityTypes.Intent abilityIntent;

    public string AbilityName
    {
        get { return abilityName; }
        set { abilityName = value; }
    }

    public string AbilityDescription
    {
        get { return abilityDescription; }
        set { abilityDescription = value; }
    }

    public int AbilityID
    {
        get { return abilityID; }
        set { abilityID = value; }
    }

    public int AbilityPower
    {
        get { return abilityPower; }
        set { abilityPower = value; }
    }

    public int ApCost
    {
        get { return apCost; }
        set { apCost = value; }
    }

    public int MpCost
    {
        get { return mpCost; }
        set { mpCost = value; }
    }

    public int AbilityRange
    {
        get { return abilityRange; }
        set { abilityRange = value; }
    }

    public virtual void ApplyCost(CharController _owner)
    {
        _owner.Stats.curAP = Mathf.Clamp(_owner.Stats.curAP - ApCost, 0, _owner.Stats.maxAP);
        _owner.Stats.curMP = Mathf.Clamp(_owner.Stats.curMP - MpCost, 0, _owner.Stats.maxMP);
    }

    public abstract void ApplyCharacterEffect(CharController _target);

    public abstract bool ValidateCost(CharController _owner);

    public virtual bool ValidateTarget(CharController _target)
    {
        return true;
    }
}
