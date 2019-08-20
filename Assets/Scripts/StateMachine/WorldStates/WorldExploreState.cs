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
    public float moveSpeed = 3.5f;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(InitBattleState),
            typeof(WorldMenuState),
            typeof(WorldMapState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        UserInputController.ResetEvents();
        base.Enter();

        movementCursor = lc.movementCursor;
        protag = lc.protag.gameObject;
        protagAnimator = protag.GetComponent<AnimationParameterController>();
        protagAgent = protag.GetComponent<NavMeshAgent>();
        _transform = protag.transform;
        _controller = protag.GetComponent<CharacterController>();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Terrain");
        lc.cameraRig.isFollowing = true;
        lc.uiController.SwitchTo("world");

        lc.protag.statusIndicator.gameObject.SetActive(false);
        lc.EnableRBs(false);
        //protagAgent.SetDestination(protagAgent.transform.position);
        //protagAgent.isStopped = false;

        InTransition = false;
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

    protected override void OnKeyDown(object sender, InfoEventArgs<KeyCode> e)
    {
        if (e.info == KeyCode.M)
            lc.ChangeState<WorldMapState>();
    }

    public override void Exit()
    {
        base.Exit();
        lc.protag.protagAgent.isStopped = true;
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
