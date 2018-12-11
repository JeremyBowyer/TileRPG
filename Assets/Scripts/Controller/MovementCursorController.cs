using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCursorController : MonoBehaviour {


    public float speed;
    public float heightIncrease;

	// Use this for initialization
	void OnEnable ()
    {
        StartCoroutine(ZoomOut());
	}

    private void OnDisable()
    {
        StopCoroutine(ZoomOut());
    }

    IEnumerator ZoomOut()
    {
        float startingHeight = transform.position.y;

        while (transform.position.y - startingHeight < heightIncrease)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        yield break;
    }
}