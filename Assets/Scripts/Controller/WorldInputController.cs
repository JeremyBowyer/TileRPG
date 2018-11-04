using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ch.sycoforge.Decal;
using System;

public class WorldInputController : MonoBehaviour {

    public float walkSpeed = 10f;
    public float jumpSpeed = 0.5f;
    public float jumpHeight = 1f;
    public bool jump;
    public Rigidbody rb;
    public Transform cameraTransform;
    public CameraController cameraController;
    public GameController gc;

    public Terrain terrain;

    void Start () {
        gc = GetComponent<GameController>();
        cameraTransform = GameObject.Find("Camera").transform;
        cameraController = GameObject.Find("CameraTarget").GetComponent<CameraController>();
        rb = GameObject.Find("Protag").GetComponent<Rigidbody>();
        jump = false;

        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
    }

	void Update () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if(x != 0 || y != 0)
        {
            Vector3 deltaMovement = AdjustMovementForCameraRotation(x, y);
            //Vector3 deltaMovement = new Vector3(x, 0, y);
            gc.protag.transform.position += deltaMovement * Time.deltaTime * walkSpeed;
        }

        if (Input.GetKeyDown("f"))
        {
            gc.grid.CreateGrid();
        }
        //if (Input.GetButtonDown("Jump") && !jump)
        //    StartCoroutine(Jump());
    }

    public IEnumerator Jump()
    {
        float originalHeight = gc.protag.transform.position.y;
        jump = true;
        yield return null;

        float maxHeight = originalHeight + jumpHeight;
        float jumpDist = maxHeight - originalHeight;

        rb.useGravity = false;
        float currentTime = 0f;
        while (gc.protag.transform.position.y < maxHeight)
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * jumpSpeed));
            float frameValue = (jumpHeight - originalHeight) * EasingEquations.Linear(0.0f, 1.0f, currentTime) + originalHeight;
            float newY = originalHeight + jumpDist * frameValue;
            gc.protag.transform.position = new Vector3(gc.protag.transform.position.x, newY, gc.protag.transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        rb.useGravity = true;

        while(gc.protag.transform.position.y > originalHeight)
        {
            yield return null;
        }
        jump = false;
        yield break;
    }

    public Vector3 AdjustMovementForCameraRotation(float x, float y)
    {
        Vector3 heading = cameraController.transform.position - cameraTransform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        Vector3 verticalDelta = new Vector3(direction.x * y, 0, direction.z * y);
        Vector3 horizontalDelta = new Vector3(direction.z * x, 0, direction.x * -x);

        return Vector3.Normalize(verticalDelta + horizontalDelta);
    }

}