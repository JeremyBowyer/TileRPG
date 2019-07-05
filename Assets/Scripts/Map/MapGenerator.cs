using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Transform tilePrefab;
	public Vector2 mapSize;

	[Range(0,1)]
	public float outlinePercent;

	void Start() {

	}

	public void GenerateMap() {

		string holderName = "Generated Map";
		if (transform.Find (holderName)) {
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		for (int x = 0; x < mapSize.x; x ++) {
			for (int y = 0; y < mapSize.y; y ++) {
				Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as Transform;
                newTile.gameObject.layer = LayerMask.NameToLayer("Ground");
                newTile.gameObject.name = "(" + x.ToString () + " , " + y.ToString () + ")";
				newTile.localScale = Vector3.one * (1-outlinePercent);
				newTile.parent = mapHolder;
			}
		}

	}

}