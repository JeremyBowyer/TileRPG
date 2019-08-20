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
        if (speed == 0)
            speed = 1f;
        if (duration == 0)
            duration = 1.5f;

        speed += speed * (GlobalRandom.GetRandom(-25, 25) / 100f);
        StartCoroutine(FloatUp());
    }

    private void OnDisable()
    {
        StopCoroutine(FloatUp());
    }

    IEnumerator FloatUp()
    {
        float offsetStart = GlobalRandom.GetRandom(-50, 50) / 100f;
        float offsetAngle = GlobalRandom.GetRandom(-10, 10) / 100f;
        transform.position = new Vector3(transform.position.x + offsetStart, transform.position.y, transform.position.z);
        while (duration > 0)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * speed + Vector3.left * offsetAngle * Time.deltaTime * speed;
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
        yield break;
    }
}
