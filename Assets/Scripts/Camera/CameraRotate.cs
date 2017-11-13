using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {

	[SerializeField]
	private float speed = 5.0f;

	public Transform _cameraTarget;
	public Camera _camera;

	// Use this for initialization
	void Start () {
		_camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (2)) {
			transform.LookAt (_cameraTarget);
			transform.RotateAround (_cameraTarget.position, Vector3.up, Input.GetAxis ("Mouse X") * speed);
		}

		float _cameraSize = _camera.orthographicSize;

		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			_cameraSize += 1;
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			_cameraSize -= 1;
		}

		_camera.orthographicSize = Mathf.Clamp (_cameraSize, 3, 9);
	}
}
