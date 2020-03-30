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
    private GameObject maladyPrefab;
    private GameObject maladyChargePrefab;
    private Dictionary<MaladyTypes.MaladyType, GameObject> maladyGOs;
    private Dictionary<MaladyTypes.MaladyType, GameObject> maladyChargeGOs;
    private IEnumerator HideCoroutine;

    [SerializeField]
    protected GameObject maladyBuildUps;
    [SerializeField]
    protected GameObject maladiesApplied;
    [SerializeField]
    protected GameObject maladiesCharged;
    [SerializeField]
    protected TextMeshProUGUI maladyNameApplied;
    [SerializeField]
    protected TextMeshProUGUI maladyNameCharged;
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

    public void Start()
    {
        popupText = Resources.Load("Prefabs/UI/PopupText") as GameObject;
        buPrefab = Resources.Load("Prefabs/UI/MaladyBuildup") as GameObject;
        maladyPrefab = Resources.Load("Prefabs/UI/Battle/CharacterMaladyApplied") as GameObject;
        maladyChargePrefab = Resources.Load("Prefabs/UI/Battle/CharacterMaladyCharged") as GameObject;

        maladyGOs = new Dictionary<MaladyTypes.MaladyType, GameObject>();
        maladyChargeGOs = new Dictionary<MaladyTypes.MaladyType, GameObject>();
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

        if (display)
            StartCoroutine(DisplayMaladyName(MaladyTypes.GetName(_malady) + " - Charged", maladyNameCharged));
    }

    public void AddMaladies(List<MaladyTypes.MaladyType> _maladies)
    {
        RemoveMaladies();
        foreach (MaladyTypes.MaladyType malady in _maladies)
        {
            AddMalady(malady);
        }
    }

    public void AddMalady(MaladyTypes.MaladyType _malady, bool display = false)
    {
        GameObject maladyGO = Instantiate(maladyPrefab, maladiesApplied.transform);
        maladyGOs[_malady] = maladyGO;
        Image icon = maladyGO.GetComponent<Image>();
        icon.sprite = MaladyTypes.GetIcon(_malady);

        if (display)
            StartCoroutine(DisplayMaladyName(MaladyTypes.GetName(_malady) + " - Applied", maladyNameApplied));
    }

    public void RemoveMaladies()
    {
        if (maladyGOs.Count == 0)
            return;

        foreach (GameObject maladyGO in maladyGOs.Values)
        {
            Destroy(maladyGO);
        }
        maladyGOs = new Dictionary<MaladyTypes.MaladyType, GameObject>();
    }

    public void RemoveMalady(MaladyTypes.MaladyType _malady)
    {
        if (maladyGOs.ContainsKey(_malady))
        {
            Destroy(maladyGOs[_malady]);
            maladyGOs.Remove(_malady);
        }
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

    public IEnumerator DisplayMaladyName(string _name, TextMeshProUGUI _nameObj, float fadeDelay = 2f)
    {
        if (!gameObject.activeInHierarchy)
            yield break;

        _nameObj.gameObject.SetActive(true);
        _nameObj.text = _name;
        yield return new WaitForSeconds(fadeDelay);
        _nameObj.gameObject.SetActive(false);
        yield return null;
    }

    public IEnumerator HideAfterDelay(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);

        yield return new WaitUntil(() => CanHide);

        if(!KeepDisplaying)
            Hide(false);
    }

}
