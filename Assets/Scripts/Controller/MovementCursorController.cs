using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCursorController : MonoBehaviour
{
    public float delay = 2f;
    public float pulseDuration = 0.25f;
    public float speed = 0.5f;

    Vector3 originalScale;

    private IEnumerator pulseCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable ()
    {
        pulseCoroutine = Pulse(delay, pulseDuration, speed);
        StartCoroutine(pulseCoroutine);
	}

    private void OnDisable()
    {
        StopCoroutine(pulseCoroutine);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Protag")
            gameObject.SetActive(false);
    }

    IEnumerator Pulse(float _delay = 2f, float _pulseDuration = 0.25f, float speed = 0.5f)
    {
        transform.localScale = originalScale;

        float countdown = _delay;

        float pulseDuration = _pulseDuration;

        float pulseMod = 1f;
        while(countdown > 0f)
        {
            float time = Time.deltaTime;

            countdown -= time;
            pulseDuration -= time;

            if(pulseDuration <= 0f)
            {
                pulseDuration = _pulseDuration;
                pulseMod *= -1f;
            }

            transform.localScale = transform.localScale + new Vector3(time, time, time) * speed * pulseMod;
            yield return null;
        }

        gameObject.SetActive(false);
        yield break;
    }
}