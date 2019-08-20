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

    public string currentPanel;

    [SerializeField] GameObject entryPrefab;
    [SerializeField] UIPanel navPanel;
    [SerializeField] UIPanel partyPanel;
    [SerializeField] UIPanel inventoryPanel;
    [SerializeField] UIPanel useItemPanel;
    [SerializeField] GameObject canvas;

    LevelController lc;
    ProtagonistController protag;
    Type currentItemType;

    UIPanel currentContentPanel;
    Dictionary<string, UIPanel> contentPanels;
    List<WorldMenuEntry> navEntries = new List<WorldMenuEntry>();
    List<GameObject> partyMemberEntries = new List<GameObject>();
    List<GameObject> useItemEntries = new List<GameObject>();
    List<GameObject> inventoryTypeEntries = new List<GameObject>();
    public int selection { get; private set; }

    void Awake()
    {
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
    }

    void Start()
    {
        navPanel.SetPosition(HideKey, false);
        contentPanels = new Dictionary<string, UIPanel>();
        contentPanels.Add("Party", partyPanel);
        contentPanels.Add("Inventory", inventoryPanel);
        contentPanels.Add("UseItem", useItemPanel);
        canvas.SetActive(false);
    }

    Tweener TogglePos(string pos, UIPanel panel)
    {
        Tweener t = panel.SetPosition(pos, true);
        t.easingControl.duration = 0.5f;
        t.easingControl.equation = EasingEquations.EaseOutQuad;
        return t;
    }

    public void Init()
    {
        protag = lc.protag;
        ShowMenu();
        LoadPartyMembers();
        LoadInventoryTypes();
    }

    public void RemoveAll()
    {
        RemoveInventoryTypes();
        RemovePartyMembers();
        RemoveUseItemEntries();
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
        useItemPanel.SetPosition(HideKey, false);
        canvas.SetActive(false);
    }

    public void ShowNavEntries(string title, Dictionary<string, UnityAction> options)
    {
        Clear();
        foreach (KeyValuePair<string, UnityAction> option in options)
        {
            GameObject goEntry = Instantiate(Resources.Load("Prefabs/UI/World Menu Entry")) as GameObject;
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

    public void LoadPartyMembers()
    {
        PartyMember protagChar = protag.character as PartyMember;
        AddPartyMember(protagChar);
        AddUseItemPartyMember(protagChar);

        foreach (PartyMember member in protag.partyMembers)
        {
            AddPartyMember(member);
            AddUseItemPartyMember(member);
        }
    }

    public void AddPartyMember(PartyMember member)
    {
        // Instantiate prefab
        GameObject goEntry = Instantiate(Resources.Load("Prefabs/UI/Party Member Entry")) as GameObject;
        goEntry.transform.SetParent(partyPanel.transform, false);
        goEntry.transform.localScale = Vector3.one;
        goEntry.SetActive(true);

        // Set Entry Fields
        PartyMenuEntry entry = goEntry.GetComponent<PartyMenuEntry>();
        entry.Reset();
        entry.mName.text = member.cName;
        entry.mClass.text = member.cClass;
        entry.mHp.text = member.stats.curHP.ToString();
        entry.mMana.text = member.stats.curMP.ToString();
        entry.mStamina.text = member.stats.curAP.ToString();
        entry.mLevel.text = member.level.ToString();
        entry.member = member;

        // Add to list, for later removal
        partyMemberEntries.Add(goEntry);
    }

    public void RefreshPartyMemberEntries()
    {
        foreach (GameObject goEntry in partyMemberEntries)
        {
            // Set Entry Fields
            PartyMenuEntry entry = goEntry.GetComponent<PartyMenuEntry>();
            PartyMember member = entry.member;
            entry.Reset();
            entry.mName.text = member.cName;
            entry.mClass.text = member.cClass;
            entry.mHp.text = member.stats.curHP.ToString();
            entry.mMana.text = member.stats.curMP.ToString();
            entry.mStamina.text = member.stats.curAP.ToString();
            entry.mLevel.text = member.level.ToString();
            entry.member = member;
        }
    }

    public void RemovePartyMembers()
    {
        foreach (GameObject entry in partyMemberEntries)
        {
            Destroy(entry);
        }
        partyMemberEntries.Clear();
    }

    public void AddUseItemPartyMember(PartyMember member)
    {
        // Instantiate prefab
        GameObject goEntry = Instantiate(Resources.Load("Prefabs/UI/Use Item Party Member Entry")) as GameObject;
        goEntry.transform.SetParent(useItemPanel.transform, false);
        goEntry.transform.localScale = Vector3.one;
        goEntry.SetActive(true);

        // Set Entry Fields
        PartyMenuEntry entry = goEntry.GetComponent<PartyMenuEntry>();
        entry.Reset();
        entry.mName.text = member.cName;
        entry.mClass.text = member.cClass;
        entry.mHp.text = member.stats.curHP.ToString();
        entry.mMana.text = member.stats.curMP.ToString();
        entry.mStamina.text = member.stats.curAP.ToString();
        entry.mLevel.text = member.level.ToString();
        entry.member = member;

        // Set On Click
        entry.setOnClick(delegate ()
        {
            Consumable item = protag.inventory.GetItemOfType(currentItemType) as Consumable;
            item.Use(member.controller);
            HideUseItemPanel();
            RefreshUseItemMemberEntries();
            RefreshPartyMemberEntries();
            RefreshInventoryEntries();
        });

        // Add to list, for later removal
        useItemEntries.Add(goEntry);
    }

    public void RefreshUseItemMemberEntries()
    {
        foreach (GameObject goEntry in useItemEntries)
        {
            // Set Entry Fields
            PartyMenuEntry entry = goEntry.GetComponent<PartyMenuEntry>();
            PartyMember member = entry.member;
            entry.Reset();
            entry.mName.text = member.cName;
            entry.mClass.text = member.cClass;
            entry.mHp.text = member.stats.curHP.ToString();
            entry.mMana.text = member.stats.curMP.ToString();
            entry.mStamina.text = member.stats.curAP.ToString();
            entry.mLevel.text = member.level.ToString();
            entry.member = member;
        }
    }

    public void RemoveUseItemEntries()
    {
        foreach(GameObject entry in useItemEntries)
        {
            Destroy(entry);
        }
        useItemEntries.Clear();
    }

    protected void LoadInventoryTypes()
    {
        RemoveInventoryTypes();
        foreach (KeyValuePair<Type, int> entry in protag.inventory.typeCount)
        {
            if(entry.Value > 0)
                AddInventoryType(entry);
        }
    }

    public void AddInventoryType(KeyValuePair<Type, int> _entry)
    {
        // Instantiate prefab
        GameObject goEntry = Instantiate(Resources.Load("Prefabs/UI/Inventory Type Entry")) as GameObject;
        goEntry.transform.SetParent(inventoryPanel.transform, false);
        goEntry.transform.localScale = Vector3.one;
        goEntry.SetActive(true);

        // Set Entry Fields
        InventoryTypeMenuEntry entry = goEntry.GetComponent<InventoryTypeMenuEntry>();
        entry.Reset();
        entry.Type = _entry.Key.ToString();
        entry.Count = "x"+_entry.Value.ToString();

        // Set On Click
        entry.setOnClick(delegate ()
        {
            ShowUseItemPanel(_entry.Key);
        });

        // Add to list, for later removal
        inventoryTypeEntries.Add(goEntry);
    }

    public void RefreshInventoryEntries()
    {
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (GameObject goEntry in inventoryTypeEntries)
        {
            // Set Entry Fields
            InventoryTypeMenuEntry entry = goEntry.GetComponent<InventoryTypeMenuEntry>();
            Type type = Type.GetType(entry.Type);
            int count = protag.inventory.typeCount[type];
            if(count <= 0)
            {

                objectsToRemove.Add(goEntry);
                continue;
            }
            entry.Type = type.ToString();
            entry.Count = "x" + count.ToString();
        }

        if(objectsToRemove.Count > 0)
        {
            foreach(GameObject goEntry in objectsToRemove)
            {
                inventoryTypeEntries.Remove(goEntry);
                Destroy(goEntry);
            }
        }

    }

    public void ShowUseItemPanel(Type type)
    {
        currentItemType = type;
        useItemPanel.SetPosition(ShowKey, true);
    }

    public void HideUseItemPanel()
    {
        useItemPanel.SetPosition(HideKey, true);
    }

    public void RemoveInventoryTypes()
    {
        foreach (GameObject entry in inventoryTypeEntries)
        {
            Destroy(entry);
        }
        inventoryTypeEntries.Clear();
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