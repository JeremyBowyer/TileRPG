using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserInputController : MonoBehaviour {

    public static event EventHandler<InfoEventArgs<GameObject>> clickEvent;
    public static event EventHandler<InfoEventArgs<GameObject>> hoverEnterEvent;
    public static event EventHandler<InfoEventArgs<GameObject>> hoverExitEvent;
    public static event EventHandler<InfoEventArgs<Point>> moveEvent;
    public static event EventHandler<InfoEventArgs<int>> fireEvent;
    public static LayerMask mouseLayer;
    private RaycastHit hit;
    private GameObject lastHit;
    private GameObject currentHit;
    private Camera _camera;
    public PauseMenu pauseMenu;
    string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" };

    private void Awake()
    {
        _camera = GameObject.Find("Camera").GetComponent<Camera>();

    }
    void Update() {

        // Pause Button
        if (Input.GetButtonDown("Pause"))
        {
            pauseMenu.paused = !pauseMenu.paused;
        }

        /* ---------------- */
        /* - Mouse Events - */
        /* ---------------- */
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        bool wasHit = Physics.Raycast(ray, out hit, 40f, 1 << mouseLayer);

        if (wasHit)
        {
            currentHit = hit.collider.gameObject;
            if (currentHit != lastHit)
            {
                if(lastHit != null)
                    hoverExitEvent(this, new InfoEventArgs<GameObject>(lastHit));
                hoverEnterEvent(this, new InfoEventArgs<GameObject>(currentHit)); // Enter should run after Exit, if highlighting tiles
                lastHit = currentHit;
            }

            if (Input.GetMouseButtonDown(0))
            {
                clickEvent(this, new InfoEventArgs<GameObject>(currentHit));
            }
        }
        else
        {
            if (lastHit != null)
                hoverExitEvent(this, new InfoEventArgs<GameObject>(lastHit));
            lastHit = null;
        }


        // Move Event
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            if (moveEvent != null)
                moveEvent(this, new InfoEventArgs<Point>(new Point(x, y)));
        }


        // Fire Event
        for (int i = 0; i < 3; ++i)
        {
            if (Input.GetButtonUp(_buttons[i]))
            {
                if (fireEvent != null)
                    fireEvent(this, new InfoEventArgs<int>(i));
            }
        }

    }
}
