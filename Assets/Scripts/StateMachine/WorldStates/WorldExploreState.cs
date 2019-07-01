using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldExploreState : WorldState
{
    private List<GameObject> startingTilesPlayer;
    private List<GameObject> startingTilesEnemy;

    // Movement fields and references
    public Camera cam;
    private CharacterController _controller;
    private GameObject protag;
    private AnimationParameterController protagAnimator;
    private Transform _transform;
    private int layerMask;
    public NavMeshAgent protagAgent;
    public GameObject movementCursor;
    public GameObject startingPlace;

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

        movementCursor = lc.movementCursor;
        protag = lc.protag.gameObject;
        protagAnimator = protag.GetComponent<AnimationParameterController>();
        protagAgent = protag.GetComponent<NavMeshAgent>();
        _transform = protag.transform;
        _controller = protag.GetComponent<CharacterController>();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Terrain");

        worldUiController.gameObject.SetActive(true);

        lc.protag.statusIndicator.gameObject.SetActive(false);
        lc.EnableRBs(true);
        //protagAgent.SetDestination(protagAgent.transform.position);
        //protagAgent.isStopped = false;

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
        lc.ChangeState<WorldMenuState>();
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
