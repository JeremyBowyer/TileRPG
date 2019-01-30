using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : State
{
    public LevelController lc;
    public CameraController cameraRig { get { return lc.cameraRig; } }
    public WorldUIController worldUiController { get { return lc.worldUiController; } }
    public SuperUIController superUiController { get { return lc.superUiController; } }
    public List<GameObject> characters { get { return lc.characters; } }

    // Methods
    protected virtual void Awake()
    {
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
    }

}
