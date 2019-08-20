using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class UserInputController : MonoBehaviour {

    public static event EventHandler<InfoEventArgs<RaycastHit>> clickEvent;
    public static event EventHandler<InfoEventArgs<GameObject>> hoverEnterEvent;
    public static event EventHandler<InfoEventArgs<GameObject>> hoverExitEvent;
    public static event EventHandler<InfoEventArgs<Point>> moveEvent;
    public static event EventHandler<InfoEventArgs<Vector3>> moveMouseEvent;
    public static event EventHandler<InfoEventArgs<int>> cancelEvent;
    public static event EventHandler<InfoEventArgs<KeyCode>> keyDownEvent;
    public static LayerMask mouseLayer;
    private static List<LayerMask> mouseLayers;
    private static int layers;
    private RaycastHit hit;
    private GameObject lastHit;
    private GameObject currentHit;
    private Camera _camera;
    public PauseMenu pauseMenu;

    private void Awake()
    {
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        mouseLayers = new List<LayerMask>();
    }

    public static void ResetEvents()
    {
        clickEvent = null;
        hoverEnterEvent = null;
        hoverExitEvent = null;
        moveEvent = null;
        moveMouseEvent = null;
        cancelEvent = null;
        keyDownEvent = null;
    }

    public static void ResetLayers()
    {
        mouseLayers.Clear();
        CalculateLayer();
    }

    void Update() {

        // Escape (cancel)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cancelEvent?.Invoke(this, new InfoEventArgs<int>(1));
        }
        
        // Key Down Event
        if (Input.GetKeyDown(KeyCode.M))
        {
            keyDownEvent?.Invoke(this, new InfoEventArgs<KeyCode>(KeyCode.M));
        }

        // Move Event
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x != 0 || y != 0)
        {
            moveEvent?.Invoke(this, new InfoEventArgs<Point>(new Point(x, y)));
        }

        /* ---------------- */
        /* - Mouse Events - */
        /* - MUST BE LAST - */
        /* ---------------- */
        if (moveMouseEvent != null)
            moveMouseEvent(this, new InfoEventArgs<Vector3>(Input.mousePosition));

        // Making the cursor invisible is used to ignore click/hover events.
        if (!Cursor.visible)
        return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        int layer = (1 << mouseLayer) | layers;

        bool wasHit = Physics.Raycast(ray, out hit, 100f, layer);

        if (wasHit && (hoverExitEvent != null || hoverEnterEvent != null || clickEvent != null))
        {
            currentHit = hit.collider.gameObject;
            if (currentHit != lastHit)
            {
                if(lastHit != null && hoverExitEvent != null)
                    hoverExitEvent(this, new InfoEventArgs<GameObject>(lastHit));
                if(hoverEnterEvent != null)
                    hoverEnterEvent(this, new InfoEventArgs<GameObject>(currentHit)); // Enter should run after Exit, if highlighting tiles
                lastHit = currentHit;
            }

            if (Input.GetMouseButtonDown(0))
            {
                clickEvent(this, new InfoEventArgs<RaycastHit>(hit));
            }
        }
        else
        {
            if (lastHit != null && hoverExitEvent != null)
                hoverExitEvent(this, new InfoEventArgs<GameObject>(lastHit));
            lastHit = null;
        }

    }

    public static void AddLayer(LayerMask layermask)
    {
        mouseLayers.Add(layermask);
        CalculateLayer();
    }

    public static void RemoveLayer(LayerMask layermask)
    {
        mouseLayers.Remove(layermask);
        CalculateLayer();
    }

    public static void CalculateLayer()
    {
        layers = 0;
        if (mouseLayers.Count > 0)
        {
            foreach (LayerMask lm in mouseLayers)
            {
                layers |= (1 << lm);
            }
        }
    }

}
