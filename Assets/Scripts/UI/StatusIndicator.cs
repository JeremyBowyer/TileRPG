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

    private GameObject popupText;

    void Start()
	{
		if (healthBarRect == null) {
			Debug.LogError ("STATUS INDICATOR: No health bar object assigned to " + gameObject.name);
		}

        popupText = Resources.Load("Prefabs/UI/PopupText") as GameObject;

    }

    public void FloatText(string text, Color color, float duration = 1.5f)
    {
        if (gameObject == null)
            return;
        GameObject popUpGO = Instantiate(popupText, gameObject.transform);
        PopupText popUp = popUpGO.GetComponent<PopupText>();
        popUp.duration = duration;
        popUp.speed = 1f;

        Text popUpText = popUpGO.GetComponent<Text>();
        popUpText.text = text;
        popUpText.color = color;

        popUpGO.SetActive(true);
    }


    public void SetHealth(int _cur, int _max)
    {
        
        if(healthBarRect != null && gameObject.activeSelf)
        {
            StartCoroutine(DepleteHealth(_cur, _max));
        }
    }

    public IEnumerator DepleteHealth(int _cur, int _max)
    {
        float currentTime = 0f;
        float speed = 5f;
        float startingScale = healthBarRect.localScale.x;
        float endingScale = (float)_cur / _max;
        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = startingScale - ((startingScale - endingScale) * EasingEquations.EaseOutCubic(0.0f, 1.0f, currentTime));
            healthBarRect.localScale = new Vector3(frameValue, healthBarRect.localScale.y, healthBarRect.localScale.z);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetAP(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        if (apBarRect != null)
            apBarRect.localScale = new Vector3(_value, apBarRect.localScale.y, healthBarRect.localScale.z);
    }

    public void SetMP(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        if (mpBarRect != null)
            mpBarRect.localScale = new Vector3(_value, mpBarRect.localScale.y, healthBarRect.localScale.z);
    }

}
