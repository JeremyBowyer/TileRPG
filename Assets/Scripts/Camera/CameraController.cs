using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // References
    private Transform followTarget;
	public Transform FollowTarget
    {
        get
        {
            if (followTarget != null)
                return followTarget;

            if (gc.cameraTarget != null)
                return gc.cameraTarget;

            return gameObject.transform;
        }

        set
        {
            followTarget = value;
        }

    }
    public GameController gc;
    private Camera _camera;

    //  Fields
    public bool isFollowing;
	private Vector3 _min, _max;
    public Vector3 Margin, Smoothing;
    private Vector3 mouseStartingPos;
    private Vector3 mouseNewPos;
    private float mouseEdgeWidth;
    private float mouseEdgeHeight;
    private float aspectRatio;
    private float boundary = 50f;

    // Camera Zoom parameters
    public float minSize;
    public float maxSize;

    private float _dragSpeed = 2f;
    private float DragSpeed
    {
        get { return _dragSpeed * _camera.orthographicSize; }
    }

    private float _edgeSpeed = 5f;
    private float EdgeSpeed
    {
        get { return _edgeSpeed * _camera.orthographicSize; }
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
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        minSize = 8f;
        maxSize = 20f;
        mouseEdgeWidth = Screen.width - boundary;
        mouseEdgeHeight = Screen.height - boundary;
        aspectRatio = mouseEdgeWidth / mouseEdgeHeight;
    }

    public void AcquireTarget()
    {
        FollowTarget = gc.cameraTarget;
    }

    public void LateUpdate()
    {
        AddTransparency();

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
            if (gc.cameraTarget == null)
                FollowTarget = gc.protag.transform;
            else
                FollowTarget = gc.cameraTarget.transform;

            if (FollowTarget != null)
            {
                //_target = gc.currentCharacter.transform;

                var x = transform.position.x;
                var y = transform.position.y;
                var z = transform.position.z;

                if (Mathf.Abs(x - FollowTarget.position.x) > Margin.x)
                    x = Mathf.Lerp(x, FollowTarget.position.x, Smoothing.x * Time.deltaTime);
                if (Mathf.Abs(y - FollowTarget.position.y) > Margin.y)
                    y = Mathf.Lerp(y, FollowTarget.position.y, Smoothing.y * Time.deltaTime);
                if (Mathf.Abs(z - FollowTarget.position.z) > Margin.z)
                    z = Mathf.Lerp(z, FollowTarget.position.z, Smoothing.z * Time.deltaTime);

                transform.position = new Vector3(x, y, z);
            }
        }

	}

    public void ScreenEdgeMovement(float x, float y)
    {

        if (Input.GetMouseButton(2))
            return;

        if(x < boundary || y < boundary || x > mouseEdgeWidth || y > mouseEdgeHeight)
        {
            isFollowing = false;
            Cursor.visible = false;

            Vector3 heading = transform.position - _camera.transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            Vector3 verticalDelta = Vector3.zero;
            Vector3 horizontalDelta = Vector3.zero;
            Vector3 normDelta;
            Vector3 moveStep;

            if (x < boundary || x > mouseEdgeWidth)
            {
                x = x - mouseEdgeWidth / 2;
                horizontalDelta = new Vector3(direction.z * x, 0, direction.x * -x);
            }

            if (y < boundary || y > mouseEdgeHeight)
            {
                y = y - mouseEdgeHeight;
                verticalDelta = new Vector3(direction.x * y, 0, direction.z * y);
            }

            normDelta = Vector3.Normalize(verticalDelta + horizontalDelta);
            normDelta.y = 0f;

            moveStep = normDelta * EdgeSpeed * Time.deltaTime;
            transform.Translate(moveStep, Space.World);
        }
        else
        {
            Cursor.visible = true;
            return;
        }
    }

    public void AddTransparency()
    {
        if (!isFollowing || FollowTarget == null)
            return;

        RaycastHit[] hits;
        float dist = Vector3.Distance(_camera.transform.position - _camera.transform.forward * 5f, FollowTarget.transform.position);
        // you can also use CapsuleCastAll()
        // TODO: setup your layermask it improve performance and filter your hits.
        hits = Physics.RaycastAll(_camera.transform.position - _camera.transform.forward * 5f, _camera.transform.forward, dist);
        foreach (RaycastHit hit in hits)
        {
            MeshRenderer R = hit.collider.GetComponent<MeshRenderer>();
            if (R == null || R.tag == "Map")
                continue;

            AutoTransparent AT = R.GetComponent<AutoTransparent>();
            if (AT == null) // if no script is attached, attach one
            {
                AT = R.gameObject.AddComponent<AutoTransparent>();
            }
            AT.BeTransparent(); // get called every frame to reset the falloff
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
