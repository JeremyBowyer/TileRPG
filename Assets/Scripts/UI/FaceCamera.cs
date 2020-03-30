using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Camera cameraToLookAt;
    public Transform cameraTarget;
    public GameObject _camera;
    CameraController camcon;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        camcon = _camera.GetComponent<CameraController>();
        cameraToLookAt = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!camcon.isFollowing)
            return;
        cameraTarget = camcon.FollowTarget;
        Vector3 v = cameraToLookAt.transform.position - cameraTarget.position;
        transform.LookAt(transform.position - v);
    }
}
