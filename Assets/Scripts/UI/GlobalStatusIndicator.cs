using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStatusIndicator : StatusIndicator
{
    [SerializeField]
    protected GameObject statusEffects;

    private GameObject effectPrefab;

    public void Start()
    {
        effectPrefab = Resources.Load("Prefabs/UI/Malady") as GameObject;
    }

    public void AddEffects(List<Malady> effects)
    {
        RemoveEffects();
        foreach (Malady effect in effects)
        {
            AddEffect(effect);
        }
    }

    public void AddEffect(Malady effect)
    {
        GameObject effectGO = Instantiate(effectPrefab, statusEffects.transform);
        Image icon = effectGO.GetComponent<Image>();
        icon.sprite = effect.icon;
    }

    public void RemoveEffects()
    {
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("EffectIcon"))
        {
            Destroy(child);
        }
    }

}
