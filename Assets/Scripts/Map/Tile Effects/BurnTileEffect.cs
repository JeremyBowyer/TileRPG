using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnTileEffect : TileEffect
{
    private int countdown;
    private GameObject go;

    public override void ApplyEffect(CharController _target)
    {
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

    public override void Tick(CharController currentCharacter)
    {
        base.Tick(currentCharacter);
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
    }

    public override void Init(Tile _tile)
    {
        base.Init(_tile);

        GameObject highlightedGO = Resources.Load("Prefabs/Grid/SelectedTile") as GameObject;
        go = Instantiate(highlightedGO, tile.WorldPosition + Vector3.up * 0.1f, Quaternion.identity, GameObject.Find("BattleGrid").transform);
        go.transform.localScale = go.transform.localScale * bc.grid.nodeDiameter;
        go.transform.rotation = Quaternion.LookRotation(bc.grid.forwardDirection, Vector3.up);

        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        mesh.material = new Material(mesh.material);
        mesh.material.SetColor("_Color", CustomColors.Fire);

        countdown = 6;
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
