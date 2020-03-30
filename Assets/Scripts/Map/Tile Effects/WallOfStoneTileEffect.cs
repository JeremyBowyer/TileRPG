using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfStoneTileEffect : TileEffect
{
    private int countdown;
    private const int MaxIterations = 1;
    private GameObject go;
    private bool wasWalkable;

    public override void OnCharEnter(CharController _target, bool queue = false)
    {
    }

    public override void OnCharExit(CharController _target, bool queue = false)
    {
    }

    public override void TurnTick(CharController previousCharacter, CharController currentCharacter)
    {
    }

    public override void RoundTick()
    {
        OnCharEnter(tile.occupant);
        if (countdown <= 0)
            RemoveEffect();
        countdown -= 1;
    }

    public override void Init(Tile _tile, Vector3 _sourceDirection, Grid _grid, Character _source)
    {
        base.Init(_tile, _sourceDirection, _grid, _source);
        wasWalkable = _tile.isWalkable;
        _tile.isWalkable = false;

        StartCoroutine(SpawnWall(_tile, _sourceDirection, _grid));

        countdown = MaxIterations;
    }

    public override void RefreshEffect()
    {
        countdown = MaxIterations;
    }

    public override void RemoveEffect()
    {
        tile.isWalkable = wasWalkable;
        //tile.node.movementCostModifier -= movementSlow;
        Destroy(go);
        base.RemoveEffect();
    }

    public IEnumerator SpawnWall(Tile _tile, Vector3 direction, Grid _grid)
    {
        GameObject wallPrefab = Resources.Load("Prefabs/Abilities/WallOfStone") as GameObject;
        go = Instantiate(wallPrefab, _tile.WorldPosition, Quaternion.identity);
        float xMod = go.GetComponent<BoxCollider>().bounds.extents.x;
        go.transform.localScale = new Vector3(go.transform.localScale.x / xMod * 0.5f, go.transform.localScale.y / xMod * 0.5f, go.transform.localScale.z / xMod * 0.5f);

        if (direction == Grid.leftDirection || direction == Grid.rightDirection)
        {
            go.transform.position += Grid.rightDirection * 0.5f;
        }
        else if (direction == Grid.forwardDirection || direction == Grid.backwardDirection)
        {
            go.transform.Rotate(new Vector3(0f, -90f, 0f));
            go.transform.position += Grid.forwardDirection * 0.5f;
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
        go.transform.parent = bc.battleRoom.transform;
        yield break;
    }

}
