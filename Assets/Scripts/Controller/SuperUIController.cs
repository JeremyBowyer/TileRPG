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

    private Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        messageText = messagePanel.GetComponent<Text>();
        backdropPanel.SetPosition(HideKey, false);
        messagePanel.SetPosition(HideKey, false);
    }

    Tweener TogglePos(string pos, UIPanel panel)
    {
        Tweener t = panel.SetPosition(pos, true);
        t.easingControl.duration = 0.5f;
        t.easingControl.equation = EasingEquations.EaseOutQuad;
        return t;
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
