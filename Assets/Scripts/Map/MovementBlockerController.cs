using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementBlockerController : MonoBehaviour
{
    public Vector3 midPoint;
    public float width = 0.5f;
    private List<GameObject> blockers;

    public void Init()
    {
        blockers = new List<GameObject>();
        Transform ap = transform.Find("AnchorPoint");
        if(ap == null)
        {
            Debug.LogError(gameObject.name + " doesn't have an anchor point, which is required for MovementBlocker.");
            return;
        }

        midPoint = ap.position;
    }

    public void SpawnBlocker(Vector3 direction)
    {
        GameObject blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blocker.GetComponent<MeshRenderer>().enabled = false;
        blocker.transform.position = midPoint + direction * width * 0.85f;
        blocker.transform.parent = transform.parent;
        blocker.gameObject.layer = LayerMask.NameToLayer("MovementBlocker");
        blocker.AddComponent<NavMeshObstacle>().carving = true;
        blocker.AddComponent<MovementBlocker>();
        blocker.name = "MovementBlocker";
        blockers.Add(blocker);

        float zScale = blocker.transform.localScale.z * 1f;
        float xScale = blocker.transform.localScale.x * 1f;

        if(direction == Vector3.forward || direction == Vector3.back)
        {
            zScale = blocker.transform.localScale.z * 0.15f;
            xScale = blocker.transform.localScale.x * 0.9f;
        } else if(direction == Vector3.left || direction == Vector3.right)
        {
            zScale = blocker.transform.localScale.z * 0.9f;
            xScale = blocker.transform.localScale.x * 0.15f;
        }

        blocker.transform.localScale = new Vector3(xScale, blocker.transform.localScale.y, zScale);
    }

    public void ClearBlockers()
    {
        foreach(GameObject blocker in blockers)
        {
            DestroyImmediate(blocker);
        }
        blockers.Clear();
    }

}
