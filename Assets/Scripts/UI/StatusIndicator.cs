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

    [SerializeField]
    protected GameObject conditionsHolder;
    protected Dictionary<ICondition, GameObject> conditions;
    protected GameObject conditionPrefab;

    protected Dictionary<MaladyTypes.MaladyType, GameObject> maladyChargeGOs;
    [SerializeField]
    protected GameObject maladyChargePrefab;
    [SerializeField]
    protected GameObject maladiesCharged;
    [SerializeField]
    protected TextMeshProUGUI displayText;

    [HideInInspector]
    public bool Displaying = false;

    public virtual void Start()
    {
        maladyChargeGOs = new Dictionary<MaladyTypes.MaladyType, GameObject>();
        conditions = new Dictionary<ICondition, GameObject>();
    }

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
            startingWidth = CustomUtils.GetRectWidth(bar) / bar.localScale.x / scale;

            innerBounds = CustomUtils.GetRectWidth(innerBoundsRect) / innerBoundsRect.localScale.x;
            outerBounds = CustomUtils.GetRectWidth(outerBoundsRect) / outerBoundsRect.localScale.x;
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

    public void AddConditions(List<ICondition> conditions)
    {
        RemoveConditions();
        foreach (ICondition condition in conditions)
        {
            AddCondition(condition);
        }
    }

    public void AddCondition(ICondition condition)
    {
        GameObject conditionGO = Instantiate(conditionPrefab, conditionsHolder.transform);
        conditions[condition] = conditionGO;
        Image icon = conditionGO.GetComponent<Image>();
        icon.sprite = condition.GetIcon();

        Tooltip tooltip = conditionGO.GetComponent<Tooltip>();
        if (tooltip != null)
        {
            tooltip.text = "<align=\"center\">" + condition.GetConditionName() + " \n</align>" + condition.GetConditionDescription();

            int roundTicks = condition.GetRemainingRoundTicks() + 1;
            int turnTicks = condition.GetRemainingTurnTicks() + 1;

            if (roundTicks > 0)
                tooltip.text = tooltip.text + " \n</align>" + "Remaining Rounds: " + roundTicks.ToString();
            if (turnTicks > 0)
                tooltip.text = tooltip.text + " \n</align>" + "Remaining Turns: " + turnTicks.ToString();
        }
    }

    public void RemoveConditions()
    {
        if (conditions == null)
            return;
        foreach (ICondition condition in conditions.Keys)
        {
            Destroy(conditions[condition]);
        }
        conditions = new Dictionary<ICondition, GameObject>();
    }

    public void RemoveCondition(ICondition condition)
    {
        if (conditions.ContainsKey(condition))
        {
            Destroy(conditions[condition]);
            conditions.Remove(condition);
        }
    }

    public void AddMaladyCharges(List<MaladyTypes.MaladyType> _maladyCharges)
    {
        RemoveMaladyCharges();
        foreach (MaladyTypes.MaladyType malady in _maladyCharges)
        {
            AddMaladyCharge(malady);
        }
    }

    public void AddMaladyCharge(MaladyTypes.MaladyType _malady, bool display = false)
    {
        GameObject maladychargeGO = Instantiate(maladyChargePrefab, maladiesCharged.transform);
        maladyChargeGOs[_malady] = maladychargeGO;
        Image icon = maladychargeGO.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = MaladyTypes.GetIcon(_malady);

        Tooltip tooltip = maladychargeGO.GetComponent<Tooltip>();
        if (tooltip != null)
            tooltip.text = "<align=\"center\">" + MaladyTypes.GetName(_malady) + " \n</align>" + MaladyTypes.GetDescription(_malady);

        if (display)
            StartCoroutine(DisplayEvent(MaladyTypes.GetName(_malady) + " - Charged", displayText));
    }

    public void RemoveMaladyCharges()
    {
        if (maladyChargeGOs.Count == 0)
            return;

        foreach (GameObject maladyChargeGO in maladyChargeGOs.Values)
        {
            Destroy(maladyChargeGO);
        }
        maladyChargeGOs = new Dictionary<MaladyTypes.MaladyType, GameObject>();
    }

    public void RemoveMaladyCharge(MaladyTypes.MaladyType _malady)
    {
        if (maladyChargeGOs.ContainsKey(_malady))
        {
            Destroy(maladyChargeGOs[_malady]);
            maladyChargeGOs.Remove(_malady);
        }
    }

    public IEnumerator DisplayEvent(string _name, TextMeshProUGUI _nameObj, float fadeDelay = 2f)
    {
        if (!gameObject.activeInHierarchy)
            yield break;

        _nameObj.gameObject.SetActive(true);
        _nameObj.text = _name;
        yield return new WaitForSeconds(fadeDelay);
        _nameObj.gameObject.SetActive(false);
        yield return null;
    }

}
