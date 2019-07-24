using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Material originalMat;
    public CharController controller;


    public void HighlightObject(Color _color)
    {
        controller = gameObject.GetComponent<CharController>();

        if (controller == null)
            return;

        if(originalMat == null)
            originalMat = controller.mesh.material;

        controller.mesh.material = new Material(originalMat);
        controller.mesh.material.color = _color;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.mesh.material = originalMat;
        }
    }
}
