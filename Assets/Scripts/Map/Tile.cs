using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {


    public List<Grid.Position> moveBlocks;
    public Grid grid;
    public Node node;
    public Vector3 WorldPosition { get { return node.worldPosition;  } }
    public float WorldX { get { return node.worldPosition.x; } }
    public float WorldY { get { return node.worldPosition.y; } }
    public bool isWalkable = true;
    private Material originalMat;
    public Projector projector;
    public Texture emptyTex;
    public Texture innerTex;
    public Texture filledTex;
    public Vector3 anchorPointWorld { get { return transform.Find("AnchorPoint").transform.position; } }
    public Vector3 anchorPointLocal { get { return transform.Find("AnchorPoint").transform.localPosition; } }

    public Dictionary<string, Color> colorDict;
    public Dictionary<string, Texture> textureDict;
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
                    Effect.OnCharEnter(character);
            }
            occupant = value;
        }
    }

    public TileEffect Effect
    {
        get { return GetComponent<TileEffect>(); }
    }

    void Awake() {
        if (projector == null)
            projector = transform.Find("AnchorPoint").transform.Find("Projector").GetComponent<Projector>();

        //anchorPointLocal = transform.Find("AnchorPoint").transform.localPosition;
        //anchorPointWorld = transform.Find("AnchorPoint").transform.position;
        moveBlocks = new List<Grid.Position>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        originalMat = projector.material;
        colorDict = new Dictionary<string, Color>();
        textureDict = new Dictionary<string, Texture>();
        colorHierarchy = new List<string>();

        emptyTex = (Texture) Resources.Load("Sprites/ThickSquare");
        innerTex = (Texture) Resources.Load("Sprites/SquareFilledSmall");
        filledTex = (Texture) Resources.Load("Sprites/SquareFilledFull");
    }

    public void FindMovementBlocks()
    {
        //Vector3 anchorPoint = transform.Find("AnchorPoint").transform.position;
        foreach (Vector3 direction in Grid.directions)
        {
            int layerMask = (1 << LayerMask.NameToLayer("MovementBlocker"));

            if (Physics.Raycast(anchorPointWorld, direction, 1f, layerMask))
            {

                if (direction == Grid.forwardDirection)
                {
                    moveBlocks.Add(Grid.Position.Front);
                }
                else if (direction == Grid.rightDirection)
                {
                    moveBlocks.Add(Grid.Position.Right);
                }
                else if (direction == Grid.backwardDirection)
                {
                    moveBlocks.Add(Grid.Position.Back);
                }
                else if (direction == Grid.leftDirection)
                {
                    moveBlocks.Add(Grid.Position.Left);
                }
                else if (direction == Grid.forwardLeftDirection)
                {
                    moveBlocks.Add(Grid.Position.FrontLeft);
                }
                else if (direction == Grid.forwardRightDirection)
                {
                    moveBlocks.Add(Grid.Position.FrontRight);
                }
                else if (direction == Grid.backwardLeftDirection)
                {
                    moveBlocks.Add(Grid.Position.BackLeft);
                }
                else if (direction == Grid.backwardRightDirection)
                {
                    moveBlocks.Add(Grid.Position.BackRight);
                }
            }
        }
    }

    public void AddColor(string set, Color color, string type)
    {
        projector.gameObject.SetActive(true);
        if(!colorDict.ContainsKey(set))
            colorDict.Add(set, color);
        if (!textureDict.ContainsKey(set))
        {
            switch (type)
            {
                case "empty":
                    textureDict.Add(set, emptyTex);
                    break;
                case "inner":
                    textureDict.Add(set, innerTex);
                    break;
                case "filled":
                    textureDict.Add(set, filledTex);
                    break;
                default:
                    textureDict.Add(set, emptyTex);
                    break;
            }
        }

        colorHierarchy.Add(set);
        ColorTile(color);
        SetProjectorTexture(textureDict[set]);
    }

    public void SetProjectorTexture(Texture _tex)
    {
        projector.material.SetTexture("_ShadowTex", _tex);
    }

    public void ColorTile(Color color)
    {
        projector.material = new Material(projector.material);
        projector.material.SetColor("_Color", color);
    }

    public void RemoveSelection(string set)
    {
        if (colorHierarchy.Contains(set))
        {
            colorHierarchy.Remove(set);
            colorDict.Remove(set);
            textureDict.Remove(set);
            if(colorHierarchy.Count > 0 && colorDict.ContainsKey(colorHierarchy[0]) && textureDict.ContainsKey(colorHierarchy[0]))
            {
                ColorTile(colorDict[colorHierarchy[0]]);
                SetProjectorTexture(textureDict[colorHierarchy[0]]);
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
