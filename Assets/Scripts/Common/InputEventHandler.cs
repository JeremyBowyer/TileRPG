using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventHandler : MonoBehaviour
{
    public static InputEventHandler instance;
    private static BattleController bc;
    private List<Outline> outlines;

    private void Awake()
    {
        instance = this;
        GameObject bcGO = GameObject.FindGameObjectWithTag("BattleController");
        if(bcGO != null)
            bc = bcGO.GetComponent<BattleController>();
        outlines = new List<Outline>();
    }

    public void UnloadTargetCharacter()
    {
        bc.TargetCharacter = null;
    }

    public void LoadTargetCharacter(CharController target)
    {
        if (target == bc.CurrentCharacter)
            return;
        bc.TargetCharacter = target;
    }

    public void OutlineCharacter(CharController character, Color _color, Outline.Mode _mode = Outline.Mode.OutlineAndSilhouette, float _width = 2f)
    {
        Outline ol = character.GetComponent<Outline>();

        if(ol == null)
            ol = character.gameObject.AddComponent<Outline>();

        ol.OutlineMode = _mode;
        ol.OutlineColor = _color;
        ol.OutlineWidth = _width;
        ol.enabled = true;
        outlines.Add(ol);
        character.outline = true;
    }

    public void RemoveOutlines()
    {
        foreach(Outline ol in outlines)
        {
            CharController character = ol.GetComponent<CharController>();

            if (character != null)
                character.outline = false;
            ol.enabled = false;
        }
        outlines.Clear();
    }

    public Tile GetTile(GameObject go, bool checkChar = false)
    {
        Tile tile = go.GetComponent<Tile>();
        
        if (tile == null && checkChar)
        {
            CharController character = go.GetComponent<CharController>();
            if (character == null || character.tile == null)
                return null;
            tile = character.tile;
        }
        return tile;
    }
}
