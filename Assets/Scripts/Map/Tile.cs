using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {

    [SerializeField]
    public List<Grid.Position> moveBlocks;

    public Grid grid;
    public Node node;
    public Vector3 WorldPosition { get { return node.worldPosition;  } }
    public float WorldX { get { return node.worldPosition.x; } }
    public float WorldY { get { return node.worldPosition.y; } }
    public bool isWalkable = true;
    private Material originalMat;
    public Projector projector;

    public Dictionary<string, Color> colorDict;
    public List<string> colorHierarchy;
    
    public CharController occupant;
    public CharController Occupant
    {
        get { return occupant; }
        set
        {
            if(Effect != null && value != null)
            {
                CharController character = value.GetComponent<CharController>();
                if (character != null)
                    Effect.ApplyEffect(character);
            }
            occupant = value;
        }
    }

    public TileEffect Effect
    {
        get { return GetComponent<TileEffect>(); }
    }

    private void OnDrawGizmos()
    {
        if (node == null)
            return;
        Gizmos.color = CustomColors.Hostile;
        foreach(Grid.Position pos in moveBlocks)
        {
            Vector3 frontLeft, frontRight, backLeft, backRight;
            frontLeft = WorldPosition + (grid.forwardDirection * grid.nodeRadius) + (grid.leftDirection * grid.nodeRadius) + (Vector3.up * 0.5f);
            frontRight = WorldPosition + (grid.forwardDirection * grid.nodeRadius) + (grid.rightDirection * grid.nodeRadius) + (Vector3.up * 0.5f);
            backLeft = WorldPosition + (grid.backwardDirection * grid.nodeRadius) + (grid.leftDirection * grid.nodeRadius) + (Vector3.up * 0.5f);
            backRight = WorldPosition + (grid.backwardDirection * grid.nodeRadius) + (grid.rightDirection * grid.nodeRadius) + (Vector3.up * 0.5f);

            if(pos == Grid.Position.Front)
            {
                Gizmos.DrawLine(frontLeft, frontRight);
            } else if(pos == Grid.Position.Left)
            {
                Gizmos.DrawLine(frontLeft, backLeft);
            } else if(pos == Grid.Position.Back)
            {
                Gizmos.DrawLine(backLeft, backRight);
            } else if(pos == Grid.Position.Right)
            {
                Gizmos.DrawLine(backRight, frontRight);
            }
        }

    }

    void Start() {
        if (projector == null)
            projector = transform.Find("AnchorPoint").transform.Find("Projector").GetComponent<Projector>();

        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        originalMat = projector.material;
        colorDict = new Dictionary<string, Color>();
        colorHierarchy = new List<string>();
    }

    public void AddColor(string set, Color color)
    {
        projector.gameObject.SetActive(true);
        if(!colorDict.ContainsKey(set))
            colorDict.Add(set, color);
        colorHierarchy.Add(set);
        ColorTile(color);
    }

    public void ColorTile(Color color)
    {
        projector.material = new Material(projector.material);
        projector.material.SetColor("_Color", color);
    }

    public void RemoveColor(string set)
    {
        if (colorHierarchy.Contains(set))
        {
            colorHierarchy.Remove(set);
            colorDict.Remove(set);
            if(colorHierarchy.Count > 0)
            {
                ColorTile(colorDict[colorHierarchy[0]]);
            }
            else
            {
                ResetColor();
            }
        }
    }

    public void ResetColor()
    {
        projector.gameObject.SetActive(false);
        projector.material = originalMat;
    }

}
