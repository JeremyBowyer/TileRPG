using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

	[SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private RectTransform apBarRect;
    [SerializeField]
    private RectTransform mpBarRect;

    void Start()
	{
		if (healthBarRect == null) {
			Debug.LogError ("STATUS INDICATOR: No health bar object referenced!");
		}
	}

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
    }

    public void SetAP(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        apBarRect.localScale = new Vector3(_value, apBarRect.localScale.y, healthBarRect.localScale.z);
    }

    public void SetMP(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        apBarRect.localScale = new Vector3(_value, mpBarRect.localScale.y, healthBarRect.localScale.z);
    }

}
