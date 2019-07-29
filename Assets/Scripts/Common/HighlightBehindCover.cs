using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightBehindCover : MonoBehaviour
{
    Camera _camera;
    CharController _controller;
    Color _color;
    Outline _ol;

    private void Start()
    {
        _camera = GameObject.Find("CameraTarget").transform.Find("Camera").GetComponent<Camera>();
        _controller = gameObject.GetComponent<CharController>();
        if (_controller == null)
            Destroy(this);

        if (_controller is EnemyController)
            _color = CustomColors.Hostile;

        if (_controller is PlayerController)
            _color = CustomColors.Heal;

        AddOutline();
    }

    public void AddOutline()
    {
        _ol = gameObject.AddComponent<Outline>();
        _ol.OutlineMode = Outline.Mode.OutlineHidden;
        _ol.OutlineColor = _color;
        _ol.OutlineWidth = 1f;

        _ol.enabled = false;
    }

    void Update()
    {
        if(_ol == null)
            AddOutline();

        RaycastHit[] hits;
        float dist = Vector3.Distance(_camera.transform.position - _camera.transform.forward * 5f, transform.position);
        // you can also use CapsuleCastAll()
        // TODO: setup your layermask it improve performance and filter your hits.

        Vector3 heading = transform.position - _camera.transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        hits = Physics.RaycastAll(_camera.transform.position - _camera.transform.forward * 5f, direction, dist);
        foreach (RaycastHit hit in hits)
        {
            /*
            if (hit.collider.gameObject == gameObject)
            {
                _ol.enabled = false;
                return;
            }
            */

            if (hit.collider.tag == "Wall")
            {
                _ol.enabled = true;
                return;
            }
        }
        _ol.enabled = false;
    }
}
