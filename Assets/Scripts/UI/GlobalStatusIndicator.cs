using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStatusIndicator : StatusIndicator
{
    [SerializeField]
    protected GameObject statusEffects;

    protected List<GameObject> maladies;
    private GameObject maladyPrefab;

    public void Start()
    {
        maladyPrefab = Resources.Load("Prefabs/UI/Malady") as GameObject;
        maladies = new List<GameObject>();
    }

    public void AddMaladies(List<Malady> maladies)
    {
        RemoveMaladies();
        foreach (Malady malady in maladies)
        {
            AddMalady(malady);
        }
    }

    public void AddMalady(Malady malady)
    {
        GameObject maladyGO = Instantiate(maladyPrefab, statusEffects.transform);
        maladies.Add(maladyGO);
        Image icon = maladyGO.GetComponent<Image>();
        icon.sprite = malady.icon;
    }

    public void RemoveMaladies()
    {
        if (maladies == null)
            return;
        foreach (GameObject maladyGO in maladies)
        {
            Destroy(maladyGO);
        }
    }

}
