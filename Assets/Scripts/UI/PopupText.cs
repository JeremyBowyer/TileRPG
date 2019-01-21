using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public float speed;
    public float duration;

    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(FloatUp());
    }

    private void OnDisable()
    {
        StopCoroutine(FloatUp());
    }

    IEnumerator FloatUp()
    {
        while (duration > 0)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * speed;
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
        yield break;
    }
}
