using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

// http://theliquidfire.com/2015/07/13/ability-menu/

public class WorldMenuPanelController : MonoBehaviour
{
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    const int MenuCount = 4;

    public string currentPanel;

    [SerializeField] GameObject entryPrefab;
    [SerializeField] UIPanel navPanel;
    [SerializeField] UIPanel partyPanel;
    [SerializeField] UIPanel inventoryPanel;
    [SerializeField] GameObject canvas;

    UIPanel currentContentPanel;
    Dictionary<string, UIPanel> contentPanels;
    List<WorldMenuEntry> navEntries = new List<WorldMenuEntry>(MenuCount);
    public int selection { get; private set; }

    void Awake()
    {

    }

    void Start()
    {
        navPanel.SetPosition(HideKey, false);
        contentPanels = new Dictionary<string, UIPanel>();
        contentPanels.Add("Party", partyPanel);
        contentPanels.Add("Inventory", inventoryPanel);
        canvas.SetActive(false);
    }

    Tweener TogglePos(string pos, UIPanel panel)
    {
        Tweener t = panel.SetPosition(pos, true);
        t.easingControl.duration = 0.5f;
        t.easingControl.equation = EasingEquations.EaseOutQuad;
        return t;
    }

    bool SetSelection(int value)
    {
        if (navEntries[value].IsLocked)
            return false;

        // Deselect the previously selected entry
        if (selection >= 0 && selection < navEntries.Count)
            navEntries[selection].IsSelected = false;

        selection = value;

        // Select the new entry
        if (selection >= 0 && selection < navEntries.Count)
            navEntries[selection].IsSelected = true;

        return true;
    }

    public void Next()
    {
        for (int i = selection + 1; i < selection + navEntries.Count; ++i)
        {
            int index = i % navEntries.Count;
            if (SetSelection(index))
                break;
        }
    }
    public void Previous()
    {
        for (int i = selection - 1 + navEntries.Count; i > selection; --i)
        {
            int index = i % navEntries.Count;
            if (SetSelection(index))
                break;
        }
    }

    void Clear()
    {
        foreach (WorldMenuEntry entry in navEntries)
            Destroy(entry.gameObject);
        navEntries.Clear();
    }

    public void ShowMenu()
    {
        canvas.SetActive(true);
    }

    public void HideMenu()
    {
        currentContentPanel.SetPosition(HideKey, false);
        canvas.SetActive(false);
    }

    public void ShowNavEntries(string title, Dictionary<string, UnityAction> options)
    {
        Clear();
        foreach (KeyValuePair<string, UnityAction> option in options)
        {
            GameObject goEntry = Instantiate(Resources.Load("Prefabs/World Menu Entry")) as GameObject;
            goEntry.transform.SetParent(navPanel.transform, false);
            goEntry.transform.localScale = Vector3.one;
            goEntry.SetActive(true);
            WorldMenuEntry entry = goEntry.GetComponent<WorldMenuEntry>();
            entry.Reset();
            entry.Title = option.Key;
            entry.setOnClick(option.Value);
            navEntries.Add(entry);
        }
        navPanel.SetPosition(ShowKey, false);
        //TogglePos(ShowKey);
    }

    public void ShowContentPanel(string panel)
    {
        if (currentContentPanel == null)
            currentContentPanel = contentPanels[panel];

        currentContentPanel.SetPosition(HideKey, true);
        currentContentPanel = contentPanels[panel];
        currentContentPanel.SetPosition(ShowKey, true);
        //Tweener t = TogglePos(HideKey, currentContentPanel);
        //t.easingControl.completedEvent += delegate (object sender, System.EventArgs e)
        //{
        //    currentContentPanel = contentPanels[panel];
        //    currentContentPanel.SetPosition(ShowKey, true);
        //};
    }

    public void AddPartyMember(PartyMember member)
    {
        GameObject goEntry = Instantiate(Resources.Load("Prefabs/Party Member Entry")) as GameObject;
        goEntry.transform.SetParent(partyPanel.transform, false);
        goEntry.transform.localScale = Vector3.one;
        goEntry.SetActive(true);
        PartyMenuEntry entry = goEntry.GetComponent<PartyMenuEntry>();
        entry.Reset();
        entry.mName.text = member.cName;
        entry.mClass.text = member.cClass;
        entry.mHp.text = member.stats.maxHealth.ToString();
        entry.mMana.text = member.stats.maxMP.ToString();
        entry.mStamina.text = member.stats.maxAP.ToString();
    }

    public void SetLocked(int index, bool value)
    {
        if (index < 0 || index >= navEntries.Count)
            return;
        navEntries[index].IsLocked = value;
        if (value && selection == index)
            Next();
    }
}