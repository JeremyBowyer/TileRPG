using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour {

    public StatusIndicator statusIndicator;
    public Text apCost;
    public Text playerName;
    public Text aiAction;
    private BattleController bc;


    // Use this for initialization
    void Awake () {
        if (apCost == null)
            Debug.LogError("No AP Cost text object assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("No StatusIndicator assigned to " + gameObject.name);

        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        aiAction = transform.Find("AIAction").GetComponent<Text>(); ;
        bc.onUnitChange += LoadStats;
        bc.onUnitChange += ShowCharUis;
        bc.onUnitChange += ToggleAIAction;
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

    public void ToggleAIAction(CharController currentCharacter)
    {
        aiAction.gameObject.SetActive(currentCharacter is EnemyController);
    }

    public void ShowCharUis(CharController currentCharacter)
    {
        foreach(GameObject charGO in bc.characters)
        {
            CharController character = charGO.GetComponent<CharController>();
            character.statusIndicator.gameObject.SetActive(true);
        }
        //currentCharacter.statusIndicator.gameObject.SetActive(false);
    }

    public void UpdateStats()
    {
        LoadStats(bc.CurrentCharacter);
    }

    public void LoadStats(CharController currentCharacter)
    {
        if(currentCharacter != null)
        {
            playerName.text = currentCharacter.Name;
            statusIndicator.SetHealth(currentCharacter.Stats.curHealth, currentCharacter.Stats.maxHealth);
            statusIndicator.SetAP(currentCharacter.Stats.curAP, currentCharacter.Stats.maxAP);
            statusIndicator.SetMP(currentCharacter.Stats.curMP, currentCharacter.Stats.maxMP);
        }
    }

}
