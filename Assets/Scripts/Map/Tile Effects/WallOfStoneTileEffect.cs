using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfStoneTileEffect : TileEffect
{
    private int countdown;
    private const int MaxIterations = 2;
    private GameObject go;
    private int movementSlow = 999;
    private bool wasWalkable;

    public override void ApplyEffect(CharController _target)
    {
    }

    public override void TurnTick(CharController _currentCharacter)
    {
    }

    public override void RoundTick()
    {
        ApplyEffect(tile.occupant);
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
    }

    public override void Init(Tile _tile)
    {
        base.Init(_tile);
        wasWalkable = _tile.isWalkable;
        _tile.isWalkable = false;
        //_tile.node.movementCostModifier += movementSlow;
        GameObject highlightedGO = Resources.Load("Prefabs/Grid/SelectedTile") as GameObject;
        go = Instantiate(highlightedGO, tile.WorldPosition + Vector3.up * 0.1f, Quaternion.identity, GameObject.Find("BattleGrid").transform);
        go.transform.localScale = go.transform.localScale * bc.grid.nodeDiameter;
        go.transform.rotation = Quaternion.LookRotation(bc.grid.forwardDirection, Vector3.up);

        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        mesh.material = new Material(mesh.material);
        mesh.material.SetColor("_Color", CustomColors.Support);

        countdown = MaxIterations;
    }

    public override void RemoveEffect()
    {
        tile.isWalkable = wasWalkable;
        //tile.node.movementCostModifier -= movementSlow;
        Destroy(go);
        base.RemoveEffect();
    }

    public override void ApplyToOccupant()
    {
    }
}
