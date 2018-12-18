using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldExploreState : State
{
    public CameraController cameraRig { get { return gc.cameraRig; } }
    public BattleUIController battleUiController { get { return gc.battleUiController; } }
    public Grid grid { get { return gc.grid; } }
    public Pathfinding pathfinder { get { return gc.pathfinder; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return gc.abilityMenuPanelController; } }
    public List<GameObject> characters { get { return gc.characters; } }

    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    // Movement fields and references
    private Transform cameraTransform;
    public Camera cam;
    private UnityEngine.CharacterController _controller;
    private GameObject protag;
    private AnimationParameterController protagAnimator;
    private Transform _transform;
    private int layerMask;
    public NavMeshAgent protagAgent;
    public GameObject movementCursor;

    // Movement parameters
    public float moveSpeed = 8f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.3f;
    public float DashDistance = 5f;
    public Vector3 Drag;

    private Vector3 _velocity;
    public bool _isGrounded = true;
    //private Transform _groundChecker;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(InitBattleState),
            typeof(WorldMenuState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();

        movementCursor = gc.movementCursor;
        protag = gc.protag.gameObject;
        protagAnimator = protag.GetComponent<AnimationParameterController>();
        protagAgent = protag.GetComponent<NavMeshAgent>();
        cameraTransform = gc._camera.transform;
        _transform = protag.transform;
        _controller = protag.GetComponent<UnityEngine.CharacterController>();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Terrain");

        StartCoroutine(gc.cameraRig.ZoomCamera(9f, 8f, 25f));
        battleUiController.gameObject.SetActive(false);
        worldUiController.gameObject.SetActive(true);

        gc.protag.statusIndicator.gameObject.SetActive(false);
        gc.EnableRBs(true);
        protagAgent.SetDestination(protagAgent.transform.position);
        protagAgent.isStopped = false;

        inTransition = false;
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        float x = e.info.x;
        float y = e.info.y;

        protagAgent.SetDestination(protagAgent.transform.position);

        protagAnimator.SetBool("running", true);
        Vector3 deltaMovement = cameraRig.AdjustMovementForCameraRotation(x, y);
        Vector3 moveStep = deltaMovement * Time.deltaTime * moveSpeed;
        _controller.Move(moveStep);
        _transform.forward = deltaMovement;
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
        protagAnimator.SetBool("running");
        protagAgent.SetDestination(e.info.point);
        movementCursor.transform.position = e.info.point;
        movementCursor.SetActive(true);
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<WorldMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
    }
}
