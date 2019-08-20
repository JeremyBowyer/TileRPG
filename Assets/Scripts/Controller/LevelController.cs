using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public BSPController bspController;

    // Directions
    public Vector3 rightDirection { get { return Vector3.right; } }
    public Vector3 leftDirection { get { return Vector3.left; } }
    public Vector3 forwardDirection { get { return Vector3.forward; } }
    public Vector3 backwardDirection { get { return Vector3.back; } }

    public void InitializeLevel()
    {
        // Assign references
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();

        protag.transform.position = GameObject.FindGameObjectWithTag("StartingTilePlayer").transform.position;
        FollowTarget(protag.transform);

        bspController = GetComponent<BSPController>();
        
        ChangeState<InitLevelState>();
    }

    public void StartBattle(BSPBattleRoom _room)
    {
        bspController.ShowRoom(_room);
        startingPos = protag.transform.position;
        ChangeState<IdleState>();
        uiController.SwitchTo("battle");
        bc.Init(_room);
    }
}
