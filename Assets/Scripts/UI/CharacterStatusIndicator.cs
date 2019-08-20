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
    [SerializeField]
    protected GameObject maladyBuildUps;

    public void Start()
    {
        popupText = Resources.Load("Prefabs/UI/PopupText") as GameObject;
        buPrefab = Resources.Load("Prefabs/UI/MaladyBuildup") as GameObject;
    }

    public void DisplayBuildUp(MaladyTypes.MaladyType type, float _cur, float _target, Action _callback)
    {
        GameObject buGO = Instantiate(buPrefab, maladyBuildUps.gameObject.transform);
        MaladyBuildUpBar buBar = buGO.GetComponent<MaladyBuildUpBar>();
        buBar.SetType(type);
        buBar.SetBU(_cur, _target, _callback);
    }

    public void FloatText(int amt, Color color, float duration = 1.5f)
    {
        if (gameObject == null)
            return;

        float fontMod = Mathf.Clamp01(amt*2 / 100f);

        int minFont = 36;
        int maxFont = 78;

        int fontSize = Mathf.RoundToInt(minFont + (maxFont - minFont) * fontMod);

        GameObject popUpGO = Instantiate(popupText, gameObject.transform);
        PopupText popUp = popUpGO.GetComponent<PopupText>();
        popUp.duration = duration;
        popUp.speed = 1f;

        Text popUpText = popUpGO.GetComponent<Text>();
        popUpText.fontSize = fontSize;
        popUpText.text = amt.ToString();
        popUpText.color = color;

        popUpGO.SetActive(true);
    }

}
