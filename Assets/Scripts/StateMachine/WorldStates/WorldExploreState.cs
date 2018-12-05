using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldExploreState : State
{
    public CameraController cameraRig { get { return gc.cameraRig; } }
    public BattleUIController battleUIController { get { return gc.uiController; } }
    public Grid grid { get { return gc.grid; } }
    public Pathfinding pathfinder { get { return gc.pathfinder; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return gc.abilityMenuPanelController; } }
    public List<GameObject> characters { get { return gc.characters; } }

    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(InitBattleState)
            };
        }
        set { }
    }
        
    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.GetComponent<WorldInputController>().enabled = true;
        StartCoroutine(gc.ZoomCamera(9f, 8f, 25f));
        gc.cameraRig._target = gc.protag.transform;
        battleUIController.gameObject.SetActive(false);
        gc.protag.statusIndicator.gameObject.SetActive(false);
        gc.EnableRBs(true);
        inTransition = false;
    }

    public override void Exit()
    {
        base.Exit();
        gc.GetComponent<WorldInputController>().enabled = false;
    }

    protected override void AddListeners()
    {
        
    }

    protected override void RemoveListeners()
    {
        
    }
}
