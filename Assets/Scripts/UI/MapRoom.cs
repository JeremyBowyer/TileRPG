using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRoom : MonoBehaviour
{
    RectTransform rect;
    Image img;
    public BSPRoom bspRoom;
    public void Init()
    {
        rect = GetComponent<RectTransform>();
        img = transform.Find("Icon").GetComponent<Image>();
    }

    public void Scale(float x, float y)
    {
        rect.sizeDelta = new Vector2(x, y);
    }

    public void SetPosition(Vector3 pos)
    {
        rect.position = pos;
    }

    public void SetIcon(Sprite icon)
    {
        if(icon == null)
        {
            img.gameObject.SetActive(false);
            return;
        }
        else
        {
            img.gameObject.SetActive(true);
            img.sprite = icon;
        }
    }
}
