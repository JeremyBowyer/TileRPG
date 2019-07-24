using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnTileEffect : TileEffect
{
    private const int MaxIterations = 2;
    private int countdown;
    private GameObject go;

    public override void ApplyEffect(CharController _target)
    {
        if (_target == null)
            return;

        BurnPlayerEffect existingEffect = _target.gameObject.GetComponent<BurnPlayerEffect>();
        if (existingEffect != null)
        {
            existingEffect.RefreshEffect();
        } else
        {
            BurnPlayerEffect pe = _target.gameObject.AddComponent<BurnPlayerEffect>();
            pe.Init(_target);
        }
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

    public override void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        base.Init(_tile, _sourceDirection, _grid);

        GameObject highlightedGO = Resources.Load("Prefabs/Grid/SelectedTile") as GameObject;
        go = Instantiate(highlightedGO, tile.WorldPosition + Vector3.up * 0.1f, Quaternion.identity);
        go.transform.rotation = Quaternion.LookRotation(bc.grid.forwardDirection, Vector3.up);
        go.transform.parent = bc.battleRoom.transform;
        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        mesh.material = new Material(mesh.material);
        mesh.material.SetColor("_Color", CustomColors.Fire);

        countdown = MaxIterations;
    }

    public override void RemoveEffect()
    {
        Destroy(go);
        base.RemoveEffect();
    }

    public override void ApplyToOccupant()
    {
        CharController occupant = tile.Occupant;
        if (occupant != null)
            ApplyEffect(occupant);
    }
}
