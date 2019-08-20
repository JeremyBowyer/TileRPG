using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseAbility {

    private string abilityName;
    private string abilityDescription;
    private Damage[] abilityDamage;
    private int apCost;
    private int mpCost;
    private float abilityRange;
    public CharController character;
    public bool diag = true;
    public bool ignoreOccupant = true;
    //public bool inProgress;
    public bool isProjectile = false;
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

    public Damage[] AbilityDamage
    {
        get { return abilityDamage; }
        set { abilityDamage = value; }
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

    public float AbilityRange
    {
        get { return abilityRange; }
        set { abilityRange = value; }
    }

    public virtual void ApplyCost(CharController _owner)
    {
        _owner.Stats.curAP = Mathf.Clamp(_owner.Stats.curAP - ApCost, 0, _owner.Stats.maxAPTemp);
        _owner.Stats.curMP = Mathf.Clamp(_owner.Stats.curMP - MpCost, 0, _owner.Stats.maxMPTemp);
    }

    public abstract void ApplyCharacterEffect(CharController _target);

    public abstract bool ValidateCost(CharController _owner);

    public abstract List<Node> GetRange();

    public virtual List<Node> PathToGetInRange(CharController _target)
    {

        List<Node> neighbors = character.bc.grid.GetNeighbors(_target.tile.node, diag, false, true);
        if (neighbors.Count == 0)
            return null;

        Node closestNode = neighbors[0];
        foreach(Node neighbor in neighbors)
        {
            float dist = character.bc.pathfinder.GetDistance(character.tile.node, neighbor);
            if (dist < character.bc.pathfinder.GetDistance(character.tile.node, closestNode))
                closestNode = neighbor;
        }


        List<Node> range = character.bc.pathfinder.FindRange(_target.tile.node, AbilityRange, diag, ignoreOccupant, true, false, true);

        List<Node> path = character.MovementAbility.GetPath(closestNode);
        List<Node> shortPath = new List<Node>();

        foreach (Node node in path)
        {
            shortPath.Add(node);
            if (range.Contains(node))
                break;
        }

        return shortPath;
    }

    public virtual Vector3[] GetPath(Vector3 _target)
    {
        return new Vector3[1]{ Vector3.zero };
    }

    public virtual bool ValidateTarget(CharController _target)
    {
        return true;
    }
}
