using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseAbility {

    private string abilityName;
    private string abilityDescription;
    private int abilityID;
    private int abilityPower;
    private int abilityCost;
    private int abilityRange;
    public Character character;
    public bool diag;
    public bool inProgress;
    public bool nextTurn;
    public int mouseLayer;

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

    public int AbilityCost
    {
        get { return abilityCost; }
        set { abilityCost = value; }
    }

    public int AbilityRange
    {
        get { return abilityRange; }
        set { abilityRange = value; }
    }
}
