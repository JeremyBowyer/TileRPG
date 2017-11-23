using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public StatusIndicator statusIndicator;
    public Text apCost;

    // Use this for initialization
    void Start () {
        if (apCost == null)
            Debug.LogError("No AP Cost text object assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("No StatusIndicator assigned to " + gameObject.name);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetApCost(int curAP, int maxAP)
    {
        apCost.text = curAP + " / " + maxAP;

        if (curAP > maxAP)
        {
            apCost.color = Color.red;
        }
        else
        {
            apCost.color = Color.green;
        }
    }

    public void SetApCost()
    {
        apCost.text = "";

    }

}
