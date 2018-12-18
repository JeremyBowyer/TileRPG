using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // References
	public Transform followTarget = null;
    private GameController gc;
    private Camera _camera;

    //  Fields
    public bool isFollowing;
	private Vector3 _min, _max;
    public Vector3 Margin, Smoothing;
    private Vector3 mouseStartingPos;
    private Vector3 mouseNewPos;

    // Camera Zoom parameters
    public float minSize;
    public float maxSize;

    private float _dragSpeed = 2f;
    private float DragSpeed
    {
        get { return _dragSpeed * _camera.orthographicSize; }
    }

    private float _rotateSpeed = 5f;

    private float CameraSize
    {
        get { return _camera.orthographicSize; }
        set { _camera.orthographicSize = Mathf.Clamp(value, minSize, maxSize); }
    }

	// Use this for initialization
	void Awake () {
		isFollowing = true;
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        minSize = 8f;
        maxSize = 20f;
    }

    public void AcquireTarget()
    {
        followTarget = gc.currentCharacter.transform;
    }

    public void LateUpdate()
    {
        // Right click to drag
        if (Input.GetMouseButtonDown(1))
        {
            mouseStartingPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            float x;
            float z;
            mouseNewPos = Input.mousePosition;

            x = mouseNewPos.x - mouseStartingPos.x;
            z = mouseNewPos.y - mouseStartingPos.y;

            Vector3 deltaMovement = AdjustMovementForCameraRotation(-x, -z);
            //Vector3 deltaMovement = new Vector3(x, 0, z);
            Vector3 moveStep = deltaMovement * DragSpeed * Time.deltaTime;

            //transform.position += moveStep;
            transform.Translate(moveStep, Space.World);

            mouseStartingPos = mouseNewPos;
            return;
        }
        else
        {
            Cursor.visible = true;
        }

        // Zoom Camera
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            CameraSize += 1;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            CameraSize -= 1;
        }

        // Rotate Camera
        if (Input.GetMouseButton(2))
        {
            _camera.transform.LookAt(transform);
            _camera.transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * _rotateSpeed);
        }

        // Follow Target
        if (isFollowing)
        {
            if (gc.currentCharacter == null)
                followTarget = gc.protag.transform;
            else
                followTarget = gc.currentCharacter.transform;

            if (followTarget != null)
            {
                //_target = gc.currentCharacter.transform;

                var x = transform.position.x;
                var y = transform.position.y;
                var z = transform.position.z;

                if (Mathf.Abs(x - followTarget.position.x) > Margin.x)
                    x = Mathf.Lerp(x, followTarget.position.x, Smoothing.x * Time.deltaTime);
                if (Mathf.Abs(y - followTarget.position.y) > Margin.y)
                    y = Mathf.Lerp(y, followTarget.position.y, Smoothing.y * Time.deltaTime);
                if (Mathf.Abs(z - followTarget.position.z) > Margin.z)
                    z = Mathf.Lerp(z, followTarget.position.z, Smoothing.z * Time.deltaTime);

                transform.position = new Vector3(x, y, z);
            }
        }

	}

    public Vector3 AdjustMovementForCameraRotation(float x, float y)
    {
        Vector3 heading = transform.position - _camera.transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        Vector3 verticalDelta = new Vector3(direction.x * y, 0, direction.z * y);
        Vector3 horizontalDelta = new Vector3(direction.z * x, 0, direction.x * -x);

        Vector3 normDelta = Vector3.Normalize(verticalDelta + horizontalDelta);
        normDelta.y = 0f;

        return normDelta;
    }

    public IEnumerator ZoomCamera(float targetSize, float _minSize, float _maxSize)
    {
        float currentSize = _camera.orthographicSize;
        float speed = 2f;
        minSize = _minSize;
        maxSize = _maxSize;

        float min = Mathf.Min(new float[] { currentSize, targetSize });
        float max = Mathf.Max(new float[] { currentSize, targetSize });

        while (!Mathf.Approximately(_camera.orthographicSize, targetSize))
        {
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + (targetSize - currentSize) * Time.deltaTime * speed, min, max);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    public IEnumerator FocusOnTarget(Transform _target)
    {

        yield break;
    }

}
