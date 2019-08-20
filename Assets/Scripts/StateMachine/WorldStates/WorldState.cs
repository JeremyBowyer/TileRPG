using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : State
{
    public LevelController lc;
    public CameraController cameraRig { get { return lc.cameraRig; } }
    public WorldUIController worldUI { get { return lc.worldUI; } }
    public SuperUIController superUI { get { return lc.superUI; } }
    public List<GameObject> characters { get { return lc.characters; } }

    // Methods
    protected override void Awake()
    {
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
    }

}
