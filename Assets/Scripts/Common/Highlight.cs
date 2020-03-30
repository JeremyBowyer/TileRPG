using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Projector projector;
    public CharController controller;
    public Color previousColor;

    public void HighlightObject(Color _color)
    {
        AssignController();
        AssignProjector();

        if (controller == null || projector == null)
            return;

        projector.enabled = true;
        projector.material.SetColor("_Color", _color);
    }

    public void RemoveHighlight()
    {
        projector.enabled = false;
    }

    public void FlashObject(Color _color)
    {
        AssignController();
        AssignProjector();

        if (controller == null || projector == null)
            return;

        StartCoroutine(Flash(_color));
    }

    public void AssignController()
    {
        if (controller != null)
            return;

        controller = GetComponentInParent<CharController>();
    }

    public void AssignProjector()
    {
        if (projector != null)
            return;

        projector = GetComponent<Projector>();
    }

    private IEnumerator Flash(Color _color)
    {
        previousColor = projector.material.GetColor("_Color");

        projector.enabled = true;
        projector.material.SetColor("_Color", _color);

        yield return new WaitForSeconds(0.25f);

        projector.material.SetColor("_Color", previousColor);
        RemoveHighlight();
    }

    private void OnDestroy()
    {
        if (projector != null)
        {
            projector.gameObject.SetActive(false);
        }
    }
}
