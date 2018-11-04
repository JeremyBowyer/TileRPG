using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Camera cameraToLookAt;
    public Transform cameraTarget;
    public GameObject _camera;


    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraToLookAt = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void Update()
    {
        CameraController camcon = _camera.GetComponent<CameraController>();
        cameraTarget = camcon._target;
        if(cameraTarget != null)
        {
            if (cameraTarget.position == transform.position)
            {
                Vector3 v = cameraToLookAt.transform.position - cameraTarget.position;
                v.x = v.z = 0.0f;
                transform.LookAt(cameraToLookAt.transform.position - v);
                transform.Rotate(0, 180, 0);
            }
            else
            {
                transform.rotation = cameraTarget.Find("CameraAngleTarget").rotation;
            }
        }
    }
}
