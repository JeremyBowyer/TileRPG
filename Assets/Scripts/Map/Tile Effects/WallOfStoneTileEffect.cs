using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfStoneTileEffect : TileEffect
{
    private int countdown;
    private const int MaxIterations = 1;
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

    public override void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        base.Init(_tile, _sourceDirection, _grid);
        wasWalkable = _tile.isWalkable;
        _tile.isWalkable = false;

        StartCoroutine(SpawnWall(_tile, _sourceDirection, _grid));

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

    public IEnumerator SpawnWall(Tile _tile, Vector3 direction, Grid _grid)
    {
        GameObject wallPrefab = Resources.Load("Prefabs/Abilities/WallOfStone") as GameObject;
        go = Instantiate(wallPrefab, _tile.WorldPosition, Quaternion.identity, GameObject.Find("BattleGrid").transform);
        float xMod = go.GetComponent<BoxCollider>().bounds.extents.x;
        go.transform.localScale = new Vector3(go.transform.localScale.x / xMod, go.transform.localScale.y / xMod, go.transform.localScale.z / xMod);

        if (direction == _grid.leftDirection || direction == _grid.rightDirection)
        {
            go.transform.position += _grid.leftDirection * 1f;
        }
        else if (direction == _grid.forwardDirection || direction == _grid.backwardDirection)
        {
            go.transform.Rotate(new Vector3(0f, -90f, 0f));
            go.transform.position += _grid.backwardDirection * 1f;
        }

        // Raise from ground
        go.transform.position += Vector3.down * 2f;
        float speed = 2f;
        while(go.transform.position.y < _tile.WorldPosition.y)
        {
            float newY = Mathf.Clamp(go.transform.position.y + Time.deltaTime * speed, go.transform.position.y, _tile.WorldPosition.y);
            go.transform.position = new Vector3(go.transform.position.x, newY, go.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

}
