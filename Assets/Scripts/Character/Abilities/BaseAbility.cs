using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseAbility {

    private string abilityName;
    private string abilityDescription;
    private Damage[] abilityDamage;
    private int hpCost;
    private int apCost;
    private int mpCost;
    private float abilityRange;
    public Sprite icon;
    public Character character;
    public CharController controller
    {
        get { return character.controller; }
    }
    public bool diag = true;
    public bool ignoreOccupant = true;
    public bool ignoreUnwalkable = true;
    public bool isProjectile = false;
    public int mouseLayer;
    public IntentTypes.Intent abilityIntent;

    public BattleController bc
    {
        get { return controller.bc; }
    }

    public LevelController lc
    {
        get { return controller.lc; }
    }

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

    public int HpCost
    {
        get { return hpCost; }
        set { hpCost = value; }
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
        _owner.Stats.curHP = Mathf.Clamp(_owner.Stats.curHP - HpCost, 0, _owner.Stats.maxHPTemp);
        _owner.Stats.curMP = Mathf.Clamp(_owner.Stats.curMP - MpCost, 0, _owner.Stats.maxMPTemp);
    }

    public virtual void ApplyBuildUp(CharController _owner)
    {

    }

    public abstract void ApplyCharacterEffect(CharController _target);

    public abstract bool ValidateCost(CharController _owner);

    public abstract List<Node> GetRange();

    public virtual List<Node> PathToGetInRange(CharController _target)
    {

        List<Node> neighbors = controller.bc.grid.GetNeighbors(_target.tile.node, diag, false, true);
        if (neighbors.Count == 0)
            return null;

        Node closestNode = neighbors[0];
        foreach(Node neighbor in neighbors)
        {
            float dist = controller.bc.pathfinder.GetDistance(controller.tile.node, neighbor);
            if (dist < controller.bc.pathfinder.GetDistance(controller.tile.node, closestNode))
                closestNode = neighbor;
        }


        List<Node> range = controller.bc.pathfinder.FindRange(_target.tile.node, AbilityRange, diag, ignoreOccupant, true, false, true);

        List<Node> path = controller.MovementAbility.GetPath(closestNode);
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
