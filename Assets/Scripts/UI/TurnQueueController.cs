using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueueController : MonoBehaviour
{
    public BattleController bc { get { return BattleController.instance; } }
    public Dictionary<CharController, TurnEntry> entries = new Dictionary<CharController, TurnEntry>();

    void Start()
    {
        bc.onBattleEnd += RemoveEntries;
        bc.onUnitDeath += OnUnitDeath;
    }

    void OnDestroy()
    {
        bc.onBattleEnd -= RemoveEntries;
        bc.onUnitDeath -= OnUnitDeath;
    }

    private void OnUnitDeath(CharController character, Damage damage)
    {
        UpdateQueue(null, null);
    }

    public void UpdateQueue(CharController previousCharacter, CharController currentCharacter)
    {
        foreach (CharController controller in entries.Keys)
        {
            if (!bc.rc.roundChars.Contains(controller))
            {
                Destroy(entries[controller].gameObject);
                entries.Remove(controller);
            }
            else
            {
                entries[controller].transform.SetSiblingIndex(bc.rc.roundChars.IndexOf(controller));
            }
        }
    }

    public void InstantiateEntries(List<GameObject> gameObjects)
    {
        RemoveEntries();
        foreach(GameObject go in gameObjects)
        {
            CharController controller = go.GetComponent<CharController>();
            if (controller != null)
                InstantiateEntry(controller);
        }
    }

    public void InstantiateEntries(List<CharController> _controllers)
    {
        RemoveEntries();
        foreach (CharController controller in _controllers)
        {
            InstantiateEntry(controller);
        }
    }

    public void InstantiateEntry(CharController controller)
    {
        GameObject entryGO = GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterTurnEntry") as GameObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        TurnEntry entry = entryGO.GetComponent<TurnEntry>();
        entry.Init(controller);
        entry.gameObject.transform.SetParent(gameObject.transform, false);
        entries[controller] = entry;
    }

    public void RemoveEntry(CharController controller)
    {
        if (entries.ContainsKey(controller))
        {
            Destroy(entries[controller].gameObject);
            entries.Remove(controller);
        }
    }

    public void RemoveEntries()
    {
        foreach(CharController controller in entries.Keys)
        {
            if(entries.ContainsKey(controller))
                Destroy(entries[controller].gameObject);
        }
        entries.Clear();
    }

}
