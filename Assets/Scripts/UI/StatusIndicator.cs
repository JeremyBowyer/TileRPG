using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

    [SerializeField]
    protected RectTransform hpBarRect;
    [SerializeField]
    protected RectTransform apBarRect;
    [SerializeField]
    protected RectTransform mpBarRect;

    [SerializeField]
    protected RectTransform hpMaxBarRect;
    [SerializeField]
    protected RectTransform apMaxBarRect;
    [SerializeField]
    protected RectTransform mpMaxBarRect;

    [SerializeField]
    protected TextMeshProUGUI hpAmount;
    [SerializeField]
    protected TextMeshProUGUI apAmount;
    [SerializeField]
    protected TextMeshProUGUI mpAmount;

    [SerializeField]
    protected Canvas canvas;

    private IEnumerator hpCoroutine;
    private IEnumerator apCoroutine;
    private IEnumerator mpCoroutine;

    private IEnumerator hpMaxCoroutine;
    private IEnumerator apMaxCoroutine;
    private IEnumerator mpMaxCoroutine;

    Vector3[] hpInnerCorners = new Vector3[4];
    Vector3[] apInnerCorners = new Vector3[4];
    Vector3[] mpInnerCorners = new Vector3[4];

    Vector3[] hpMaxCorners = new Vector3[4];
    Vector3[] apMaxCorners = new Vector3[4];
    Vector3[] mpMaxCorners = new Vector3[4];

    float hpInnerWidth;
    float apInnerWidth;
    float mpInnerWidth;

    float hpMaxWidth;
    float apMaxWidth;
    float mpMaxWidth;

    [HideInInspector]
    public bool Displaying = false;

    public void SetCurrentHP(int _cur, int _tempMax, int _max, bool animate = true)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (hpAmount != null)
            hpAmount.text = _cur.ToString();

        if(hpBarRect != null)
        {
            if (hpCoroutine != null)
                StopCoroutine(hpCoroutine);

            hpCoroutine = ScaleStatBar(hpBarRect, _cur, _max, animate);
            StartCoroutine(hpCoroutine);
        }

        if(hpMaxBarRect != null)
        {
            if (hpMaxCoroutine != null)
                StopCoroutine(hpMaxCoroutine);

            hpMaxCoroutine = ScaleMaxStatBar(hpMaxBarRect, _tempMax, _max, animate);
            StartCoroutine(hpMaxCoroutine);
        }
    }

    public void SetCurrentAP(int _cur, int _tempMax, int _max, bool animate = true)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (apAmount != null)
            apAmount.text = _cur.ToString();

        if (apBarRect != null)
        {
            if (apCoroutine != null)
                StopCoroutine(apCoroutine);

            apCoroutine = ScaleStatBar(apBarRect, _cur, _max, animate);
            StartCoroutine(apCoroutine);
        }

        if (apMaxBarRect != null)
        {
            if (apMaxCoroutine != null)
                StopCoroutine(apMaxCoroutine);

            apMaxCoroutine = ScaleMaxStatBar(apMaxBarRect, _tempMax, _max, animate);
            StartCoroutine(apMaxCoroutine);
        }
    }

    public void SetCurrentMP(int _cur, int _tempMax, int _max, bool animate = true)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (mpAmount != null)
            mpAmount.text = _cur.ToString();

        if (mpBarRect != null)
        {
            if (mpCoroutine != null)
                StopCoroutine(mpCoroutine);

            mpCoroutine = ScaleStatBar(mpBarRect, _cur, _max, animate);
            StartCoroutine(mpCoroutine);
        }

        if (mpMaxBarRect != null)
        {
            if (mpMaxCoroutine != null)
                StopCoroutine(mpMaxCoroutine);

            mpMaxCoroutine = ScaleMaxStatBar(mpMaxBarRect, _tempMax, _max, animate);
            StartCoroutine(mpMaxCoroutine);
        }
    }

    public IEnumerator ScaleStatBar(RectTransform bar, int _cur, int _max, bool animate = true)
    {
        Displaying = true;
        float currentTime = 0f;
        float speed = 1f;
        float startingScale = bar.localScale.x;
        float endingScale = (float)_cur / 100f;

        if (!animate)
        {
            bar.localScale = new Vector3(endingScale, bar.localScale.y, bar.localScale.z);
            yield break;
        }

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = startingScale + ((endingScale - startingScale) * EasingEquations.EaseOutCubic(0.0f, 1.0f, currentTime));
            bar.localScale = new Vector3(frameValue, bar.localScale.y, bar.localScale.z);
            yield return new WaitForEndOfFrame();
        }

        Displaying = false;
    }

    public virtual IEnumerator ScaleMaxStatBar(RectTransform bar, int _cur, int _max, bool animate = true)
    {
        Displaying = true;

        yield return new WaitForEndOfFrame();
        float currentTime = 0f;
        float speed = 1f;

        Transform innerBoundsT, outerBoundsT;

        innerBoundsT = bar.transform.Find("InnerBounds");
        outerBoundsT = bar.transform.Find("OuterBounds");

        if (innerBoundsT == null || outerBoundsT == null)
            goto EndCoroutine;

        RectTransform innerBoundsRect, outerBoundsRect;

        innerBoundsRect = innerBoundsT.GetComponent<RectTransform>();
        outerBoundsRect = outerBoundsT.GetComponent<RectTransform>();

        if (innerBoundsRect == null || outerBoundsRect == null)
            goto EndCoroutine;


        float startingWidth = 0f, innerBounds = 0f, outerBounds = 0f, baseWidth = 0f, endingWidth = 0f;
        float scale = canvas.scaleFactor;
        if(this is GlobalStatusIndicator)
        {
            startingWidth = CustomUtils.GetWidth(bar) / bar.localScale.x / scale;

            innerBounds = CustomUtils.GetWidth(innerBoundsRect) / innerBoundsRect.localScale.x;
            outerBounds = CustomUtils.GetWidth(outerBoundsRect) / outerBoundsRect.localScale.x;
            baseWidth = outerBounds - innerBounds;
            //endingWidth = (baseWidth + innerBounds * ((float)_cur / _max)) * 2f / scale;
            endingWidth = (baseWidth + innerBounds * ((float)_cur / 100f)) / scale;
        } else if (this is CharacterStatusIndicator)
        {
            startingWidth = bar.rect.width / scale;

            innerBounds = innerBoundsRect.rect.width / innerBoundsRect.localScale.x;
            outerBounds = outerBoundsRect.rect.width / outerBoundsRect.localScale.x;
            baseWidth = outerBounds - innerBounds;
            //endingWidth = (baseWidth + innerBounds * ((float)_cur / _max)) / scale;
            endingWidth = (baseWidth + innerBounds * ((float)_cur / 100f)) / scale;
        }

        if (!animate)
        {
            bar.sizeDelta = new Vector2(endingWidth, bar.sizeDelta.y);
            goto EndCoroutine;
        }

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = startingWidth + ((endingWidth - startingWidth) * EasingEquations.EaseOutCubic(0.0f, 1.0f, currentTime));
            bar.sizeDelta = new Vector2(frameValue, bar.sizeDelta.y);
            yield return new WaitForEndOfFrame();
        }

        EndCoroutine:
        Displaying = false;
        yield break;
    }

}
