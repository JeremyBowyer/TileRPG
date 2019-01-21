using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {
    
    public Grid grid;
    public Node node;
    public Vector3 WorldPosition { get { return node.worldPosition;  } }
    public float WorldX { get { return node.worldPosition.x; } }
    public float WorldY { get { return node.worldPosition.y; } }
    public TileEffect effect;
    public CharController occupant;

    public CharController Occupant
    {
        get { return occupant; }
        set
        {
            if(effect != null)
            {
                CharController character = value.GetComponent<CharController>();
                if (character != null)
                    effect.ApplyEffect(character);
            }
            occupant = value;
        }
    }
    public bool isWalkable = true;

	void Start() {
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
    }

}
