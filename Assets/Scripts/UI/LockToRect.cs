using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToRect : MonoBehaviour
{
    public RectTransform target;
    private RectTransform thisRect;
    public enum LockToRectPosition { Top, Bottom, Left, Right }
    public LockToRectPosition pos;
    public float offset;

    public void Lock()
    {
        if (target == null)
            return;

        thisRect = GetComponent<RectTransform>();
        if (thisRect == null)
            Debug.LogError(gameObject.name + " must be attached to object with a rect transform.");

        SetPosition();
    }

    public void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        Vector3 targetPos = target.transform.position;
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        float height = (corners[1].y - corners[0].y) / 2f;
        float width = (corners[3].x - corners[0].x) / 2f;

        switch (pos)
        {
            case LockToRectPosition.Top:
                gameObject.transform.position = corners[1] + Vector3.right * width + Vector3.up * offset;
                break;
            case LockToRectPosition.Bottom:
                gameObject.transform.position = corners[0] + Vector3.right * width + Vector3.down * offset;
                break;
            case LockToRectPosition.Left:
                gameObject.transform.position = corners[0] + Vector3.up * height + Vector3.left * offset;
                break;
            case LockToRectPosition.Right:
                gameObject.transform.position = corners[3] + Vector3.up * height + Vector3.right * offset;
                break;
        }
    }
}
