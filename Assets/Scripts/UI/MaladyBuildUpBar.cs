using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaladyBuildUpBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform buildBarRect;
    [SerializeField]
    private Image buildBarImage;
    [SerializeField]
    private Image iconImg;

    public void SetType(MaladyTypes.MaladyType type)
    {
        iconImg.sprite = MaladyTypes.GetIcon(type);
        buildBarImage.color = MaladyTypes.GetColor(type);
    }

    public void SetBU(float _cur, float _target, Action _callback, float _max = 100f)
    {
        if (buildBarRect != null && buildBarRect.gameObject.activeSelf && gameObject.activeSelf)
        {
            buildBarRect.localScale = new Vector3(_cur, buildBarRect.localScale.y, buildBarRect.localScale.z);
            StartCoroutine(FillBar(buildBarRect, _target, _max, _callback));
        }
    }

    public IEnumerator FillBar(RectTransform bar, float _target, float _max, Action _callback)
    {
        float currentTime = 0f;
        float speed = 1f;
        float startingScale = bar.localScale.x / 100f;
        float endingScale = _target / _max;
        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = startingScale - ((startingScale - endingScale) * EasingEquations.EaseOutCubic(0.0f, 1.0f, currentTime));

            bar.localScale = new Vector3(frameValue, bar.localScale.y, bar.localScale.z);
            yield return new WaitForEndOfFrame();
        }

        if (_target >= _max)
            _callback?.Invoke();

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
