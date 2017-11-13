using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public Tile tile;

	public void Place (Tile targetTile)
    {
        float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.z * 2;
        Vector3 _targetPos = targetTile.transform.position;
        transform.position = targetTile.transform.position + new Vector3(0, _height, 0);
        tile = targetTile;
    }
}
