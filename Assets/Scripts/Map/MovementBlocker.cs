using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBlocker : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (gameObject.activeSelf)
       {
            BoxCollider bc = GetComponent<BoxCollider>();
            Gizmos.color = CustomColors.Fire;
            Gizmos.DrawCube(transform.position, bc.bounds.size);
        }
    }
}
