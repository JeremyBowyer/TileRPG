using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueueController : MonoBehaviour
{
    public BattleController bc;
    public List<GameObject> slots;
    public GameObject hideSlot;
    public List<TurnEntry> entries;

    public void UpdateQueue(CharController character)
    {
        int maxSlots = slots.Count;
        for(int i=0; i < bc.rc.roundChars.Count; i++)
        {
            if (i < maxSlots)
            {
                StartCoroutine(MoveEntryToSlot(bc.rc.roundChars[i].turnEntry, slots[i]));
            }
        }
    }

    public void InstantiateEntry(CharController controller)
    {
        GameObject entryGO = GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterTurnEntry") as GameObject, hideSlot.transform.position, Quaternion.identity) as GameObject;
        TurnEntry entry = entryGO.GetComponent<TurnEntry>();
        entry.Init(controller);
        entry.gameObject.transform.SetParent(hideSlot.transform, false);
        entries.Add(entry);
        HideEntry(entry);
    }

    public GameObject FindEmptySlot()
    {
        foreach(GameObject slot in slots)
        {
            if (slot.transform.childCount == 0)
                return slot;
        }
        return new GameObject();
    }

    public void HideEntry(CharController controller)
    {
        foreach(TurnEntry entry in entries)
        {
            if (entry.character = controller)
                HideEntry(entry);
        }
    }

    public void HideEntry(TurnEntry entry)
    {
        StartCoroutine(MoveEntryToSlot(entry, hideSlot));
    }

    public IEnumerator MoveEntryToSlot(TurnEntry entry, GameObject slot)
    {
        if (entry.gameObject.transform.parent == slot.transform)
            yield break;

        entry.gameObject.transform.SetParent(slot.transform);

        float currentTime = 0f;
        float speed = 2f;

        Vector2 startingPos = new Vector2(entry.transform.position.y, entry.transform.position.x);
        Vector2 endingPos = new Vector2(slot.transform.position.y, slot.transform.position.x);

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + Time.deltaTime * speed);
            float frameValue = EasingEquations.EaseInOutExpo(0.0f, 1.0f, currentTime);
            Vector2 framePos = MathCurves.Parabola(startingPos, endingPos, 50f, frameValue);
            Vector3 entryPos = new Vector3(framePos.y, framePos.x, 0);
            entry.transform.position = entryPos;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    public void EndBattle()
    {
        foreach(TurnEntry entry in entries)
        {
            Destroy(entry.gameObject);
        }
        entries.Clear();
    }

}
