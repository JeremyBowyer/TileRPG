using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldInputController : MonoBehaviour {

    private Transform cameraTransform;
    private CameraController cameraController;
    private CharacterController _controller;
    private GameObject protag;
    private Transform _transform;

    public float moveSpeed = 8f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.3f;
    public float DashDistance = 5f;
    public Vector3 Drag;

    private Vector3 _velocity;
    public bool _isGrounded = true;
    private Transform _groundChecker;

    // Use this for initialization
    void Start()
    {
        protag = GameObject.Find("Protag");
        cameraTransform = GameObject.Find("Camera").transform;
        cameraController = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        _transform = protag.transform;
        _controller = protag.GetComponent<CharacterController>();
        _groundChecker = protag.transform.Find("GroundChecker");
    }

    void Update()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Character"));
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, layerMask, QueryTriggerInteraction.UseGlobal);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x != 0 || y != 0)
        {
            Vector3 deltaMovement = AdjustMovementForCameraRotation(x, y);
            Vector3 moveStep = deltaMovement * Time.deltaTime * moveSpeed;
            _controller.Move(moveStep);
            _transform.forward = deltaMovement;
            /*
            horizontalTarget = transform.position + moveStep;
            transform.LookAt(horizontalTarget);
            */
        }

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Debug.Log("Jump");
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        if (Input.GetKeyDown("f"))
        {
            Debug.Log("Dash");
            _velocity += Vector3.Scale(_transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
        }


        _velocity.y += Gravity * Time.deltaTime;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    public Vector3 AdjustMovementForCameraRotation(float x, float y)
    {
        Vector3 heading = cameraController.transform.position - cameraTransform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        Vector3 verticalDelta = new Vector3(direction.x * y, 0, direction.z * y);
        Vector3 horizontalDelta = new Vector3(direction.z * x, 0, direction.x * -x);

        Vector3 normDelta = Vector3.Normalize(verticalDelta + horizontalDelta);
        normDelta.y = 0f;

        return normDelta;
    }

}