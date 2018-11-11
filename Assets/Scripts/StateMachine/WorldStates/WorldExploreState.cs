using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldExploreState : State
{
    protected GameController gc;
    public CameraController cameraRig { get { return gc.cameraRig; } }
    public BattleUIController battleUIController { get { return gc.uiController; } }
    public Grid grid { get { return gc.grid; } }
    public Pathfinding pathfinder { get { return gc.pathfinder; } }
    public Node node { get { return gc.node; } set { gc.node = value; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return gc.abilityMenuPanelController; } }
    public List<GameObject> characters { get { return gc.characters; } }

    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    protected virtual void Awake()
    {
        gc = GetComponent<GameController>();
    }

    public override void Enter()
    {
        base.Enter();
        gc.GetComponent<WorldInputController>().enabled = true;
        StartCoroutine(gc.ZoomCamera(9f));
        StartCoroutine(Init());
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


    IEnumerator Init()
    {
        gc.cameraRig._target = gc.protag.transform;
        battleUIController.gameObject.SetActive(false);
        gc.protag.statusIndicator.gameObject.SetActive(false);

        gc.EnableRBs(true);
        
        /*
        foreach (GameObject enemy in gc.worldEnemies)
        {

            enemy.GetComponent<DetectPlayer>().enabled = true;
        }
        */
        yield break;
    }
}
