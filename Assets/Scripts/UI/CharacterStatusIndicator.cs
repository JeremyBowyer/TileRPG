using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusIndicator : StatusIndicator
{
    private GameObject popupText;
    private GameObject buPrefab;
    private IEnumerator HideCoroutine;

    [SerializeField]
    protected GameObject maladyBuildUps;
    [SerializeField]
    private GameObject holder;

    [HideInInspector]
    public List<MaladyBuildUpBar> buBars;

    public bool KeepDisplaying;
    public bool CanHide
    {
        get
        {
            bool buBarsDisplaying = false;

            foreach(MaladyBuildUpBar buBar in buBars)
            {
                if (buBar.Displaying)
                {
                    buBarsDisplaying = true;
                    break;
                }
            }

            return !Displaying && !buBarsDisplaying;
        }
    }

    public override void Start()
    {
        base.Start();
        popupText = Resources.Load("Prefabs/UI/PopupText") as GameObject;
        buPrefab = Resources.Load("Prefabs/UI/MaladyBuildup") as GameObject;
        conditionPrefab = Resources.Load("Prefabs/UI/Battle/CharacterCondition") as GameObject;
        maladyChargePrefab = Resources.Load("Prefabs/UI/Battle/CharacterMaladyCharged") as GameObject;
        
        buBars = new List<MaladyBuildUpBar>();

        if(holder == null)
            holder = transform.Find("Holder").gameObject;
    }

    public void DisplayBuildUp(MaladyTypes.MaladyType type, float _cur, float _target, Action _callback = null)
    {
        if (!gameObject.activeInHierarchy)
            return;

        GameObject buGO = Instantiate(buPrefab, maladyBuildUps.gameObject.transform);
        MaladyBuildUpBar buBar = buGO.GetComponent<MaladyBuildUpBar>();
        buBars.Add(buBar);
        buBar.SetType(type);
        buBar.SetBU(_cur, _target, _callback);
    }

    public void FloatText(int amt, Color color, float duration = 1.5f)
    {
        if (gameObject == null || !gameObject.activeSelf)
            return;

        float fontMod = Mathf.Clamp01(amt*2 / 100f);

        int minFont = 56;
        int maxFont = 96;

        int fontSize = Mathf.RoundToInt(minFont + (maxFont - minFont) * fontMod);

        GameObject popUpGO = Instantiate(popupText, gameObject.transform);
        PopupText popUp = popUpGO.GetComponent<PopupText>();
        popUp.duration = duration;
        popUp.speed = 1f;

        TextMeshPro popUpText = popUpGO.GetComponent<TextMeshPro>();
        popUpText.fontSize = fontSize;
        popUpText.text = amt.ToString();
        popUpText.color = color;

        popUpGO.SetActive(true);
    }

    public void Show()
    {
        KeepDisplaying = true;
        holder.SetActive(true);
    }

    public void Hide(bool wait = true)
    {
        KeepDisplaying = false;
        if (wait)
        {
            if (HideCoroutine != null)
                StopCoroutine(HideCoroutine);
            HideCoroutine = HideAfterDelay(delay: 0f);
            StartCoroutine(HideCoroutine);
        }
        else
            holder.SetActive(false);
    }

    public void ShowTemp(float delay = 3f)
    {
        holder.SetActive(true);

        if (HideCoroutine != null)
            StopCoroutine(HideCoroutine);
        HideCoroutine = HideAfterDelay(delay);
        StartCoroutine(HideCoroutine);
    }

    public IEnumerator HideAfterDelay(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);

        yield return new WaitUntil(() => CanHide);

        if(!KeepDisplaying)
            Hide(false);
    }

}
