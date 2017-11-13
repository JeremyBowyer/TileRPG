using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserInputController : MonoBehaviour {

    public static event EventHandler<InfoEventArgs<GameObject>> clickEvent;
    public static event EventHandler<InfoEventArgs<Point>> moveEvent;
    public static event EventHandler<InfoEventArgs<int>> fireEvent;
    private RaycastHit hit;
    private Camera _camera;

    private void Awake()
    {
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
    }
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20f))
            {
                clickEvent(this, new InfoEventArgs<GameObject>(hit.collider.gameObject));
            }
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<Point>(new Point(x, y)));
        }
    }
}
