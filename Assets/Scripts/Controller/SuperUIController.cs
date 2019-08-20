using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperUIController : MonoBehaviour
{
    const string ShowKey = "Show";
    const string HideKey = "Hide";

    [SerializeField] UIPanel backdropPanel;
    [SerializeField] UIPanel messagePanel;
    [SerializeField] UIPanel mapPanel;

    public MapController uiMapController;

    private Text messageText;

    void Start()
    {
        messageText = messagePanel.GetComponent<Text>();
        backdropPanel.SetPosition(HideKey, false);
        messagePanel.SetPosition(HideKey, false);
    }

    Tweener TogglePos(string pos, UIPanel panel, float duration = 0.5f)
    {
        Tweener t = panel.SetPosition(pos, true);
        t.easingControl.duration = duration;
        t.easingControl.equation = EasingEquations.EaseOutQuad;
        return t;
    }

    public void ShowMap()
    {
        uiMapController.UpdateMapUI();
        TogglePos(ShowKey, mapPanel, 0.25f);
    }

    public void HideMap()
    {
        uiMapController.UpdateMapUI();
        TogglePos(HideKey, mapPanel, 0.25f);
    }

    public void ToggleMap()
    {
        uiMapController.UpdateMapUI();
        string key = mapPanel.CurrentPosition.name == HideKey ? ShowKey : HideKey;
        TogglePos(key, mapPanel, 0.25f);
    }

    public void ShowMessage(string msg, float duration)
    {
        messageText.text = msg;
        backdropPanel.SetPosition(ShowKey, true);
        messagePanel.SetPosition(ShowKey, true);

        StartCoroutine(HideMessage(duration));
    }

    public IEnumerator HideMessage(float timeout)
    {
        while (timeout > 0)
        {
            yield return new WaitForEndOfFrame();
            timeout -= Time.deltaTime;
        }
        backdropPanel.SetPosition(HideKey, true);
        messagePanel.SetPosition(HideKey, true);
    }

}
