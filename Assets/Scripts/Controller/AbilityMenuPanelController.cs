using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// http://theliquidfire.com/2015/07/13/ability-menu/

public class AbilityMenuPanelController : MonoBehaviour
{
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    const string EntryPoolKey = "AbilityMenuPanel.Entry";
    const int MenuCount = 4;

    [SerializeField] GameObject entryPrefab;
    [SerializeField] Text titleLabel;
    [SerializeField] UIPanel panel;
    [SerializeField] GameObject canvas;
    List<AbilityMenuEntry> menuEntries = new List<AbilityMenuEntry>(MenuCount);
    public int selection { get; private set; }

    void Awake()
    {

    }

    void Start()
    {
        panel.SetPosition(HideKey, false);
        canvas.SetActive(false);
    }

    Tweener TogglePos(string pos)
    {
        Tweener t = panel.SetPosition(pos, true);
        t.easingControl.duration = 0.5f;
        t.easingControl.equation = EasingEquations.EaseOutQuad;
        return t;
    }

    bool SetSelection(int value)
    {
        if (menuEntries[value].IsLocked)
            return false;

        // Deselect the previously selected entry
        if (selection >= 0 && selection < menuEntries.Count)
            menuEntries[selection].IsSelected = false;

        selection = value;

        // Select the new entry
        if (selection >= 0 && selection < menuEntries.Count)
            menuEntries[selection].IsSelected = true;

        return true;
    }

    public void Next()
    {
        for (int i = selection + 1; i < selection + menuEntries.Count; ++i)
        {
            int index = i % menuEntries.Count;
            if (SetSelection(index))
                break;
        }
    }
    public void Previous()
    {
        for (int i = selection - 1 + menuEntries.Count; i > selection; --i)
        {
            int index = i % menuEntries.Count;
            if (SetSelection(index))
                break;
        }
    }

    void Clear()
    {
        foreach (AbilityMenuEntry entry in menuEntries)
            Destroy(entry.gameObject);
        menuEntries.Clear();
    }

    public void Show(string title, Dictionary<string, UnityAction> options)
    {
        canvas.SetActive(true);
        Clear();
        titleLabel.text = title;
        foreach(KeyValuePair<string, UnityAction> option in options)
        {
            GameObject goEntry = Instantiate(Resources.Load("Prefabs/Ability Menu Entry")) as GameObject;
            goEntry.transform.SetParent(panel.transform, false);
            goEntry.transform.localScale = Vector3.one;
            goEntry.SetActive(true);
            AbilityMenuEntry entry = goEntry.GetComponent<AbilityMenuEntry>();
            entry.Reset();
            entry.Title = option.Key;
            entry.setOnClick(option.Value);
            menuEntries.Add(entry);
        }
        TogglePos(ShowKey);
    }

    public void SetLocked(int index, bool value)
    {
        if (index < 0 || index >= menuEntries.Count)
            return;
        menuEntries[index].IsLocked = value;
        if (value && selection == index)
            Next();
    }

    public void Hide()
    {
        Tweener t = TogglePos(HideKey);
        t.easingControl.completedEvent += delegate (object sender, System.EventArgs e)
        {
            if (panel.CurrentPosition == panel[HideKey])
            {
                Clear();
                canvas.SetActive(false);
            }
        };
    }

}