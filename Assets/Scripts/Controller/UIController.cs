using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public SuperUIController superUI;
    public BattleUIController battleUI;
    public WorldUIController worldUI;

    public void SwitchTo(string state)
    {
        switch (state)
        {
            case "battle":
                worldUI.gameObject.SetActive(false);
                battleUI.gameObject.SetActive(true);
                break;
            case "world":
                battleUI.gameObject.SetActive(false);
                worldUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
