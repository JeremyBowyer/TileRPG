using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class LevelController : GameController
{
    // References
    public GameObject movementCursor;
    public UIController uiController;
    public WorldMenuPanelController worldMenuPanelController;
    public WorldUIController worldUI { get { return uiController.worldUI; } }
    public SuperUIController superUI { get { return uiController.superUI; } }
    public BattleUIController battleUI { get { return uiController.battleUI; } }
    public EnemyController battleInitiator;
    public BattleController bc;
    public Vector3 startingPos;
    [HideInInspector]
    public BSPController bspController;
    public Level level;
    public Roster globalEnemyGroup;
    public Camera miniMapCamera;
    public GameObject miniMapPlayer;
    public GameObject mapHolder;

    public static LevelController instance;

    // Directions
    public static Vector3 rightDirection         { get { return Vector3.right; } }
    public static Vector3 leftDirection          { get { return Vector3.left; } }
    public static Vector3 forwardDirection       { get { return Vector3.forward; } }
    public static Vector3 backwardDirection      { get { return Vector3.back; } }
    public static Vector3 forwardLeftDirection   { get { return new Vector3(-0.5f, 0f, 0.5f); } }
    public static Vector3 forwardRightDirection  { get { return new Vector3(0.5f, 0f, 0.5f); } }
    public static Vector3 backwardLeftDirection  { get { return new Vector3(-0.5f, 0f, -0.5f); } }
    public static Vector3 backwardRightDirection { get { return new Vector3(0.5f, 0f, -0.5f); } }

    public void Awake()
    {
        instance = this;
    }

    public void InitializeLevel()
    {
        AssignReferences();

        float mapSize = Mathf.Max(new float[] { bspController.gridSize.x, bspController.gridSize.y });
        if (miniMapCamera != null)
            miniMapCamera.orthographicSize = mapSize / 2f + 5f;

        if (miniMapPlayer != null)
        {
            Instantiate(miniMapPlayer, protag.transform);
        }

        ChangeState<InitLevelState>();
    }

    public override void AssignReferences()
    {
        base.AssignReferences();
        level = new KeepLevel();
        InstantiateProtagonist();
        bspController = GetComponent<BSPController>();
    }

    public void StartBattle(KeepBattleRoom _room)
    {
        //bspController.ShowRoom(_room);
        startingPos = protag.transform.position;
        ChangeState<IdleState>();
        bc.InitBattle(_room);
    }
}
