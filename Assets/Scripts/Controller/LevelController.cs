using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : GameController
{
    // References
    public GameObject movementCursor;
    public WorldMenuPanelController worldMenuPanelController;
    public WorldUIController worldUiController;
    public EnemyController battleInitiator;

    void Start()
    {
        // Assign references
        protag = GameObject.FindGameObjectWithTag("Protag").GetComponent<ProtagonistController>();
        cameraRig = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();

        cameraRig.FollowTarget = protag.transform;
        cameraTarget = protag.transform;

        ChangeState<InitLevelState>();
    }
}
