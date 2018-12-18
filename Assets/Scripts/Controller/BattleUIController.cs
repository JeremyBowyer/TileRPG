using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour {

    public StatusIndicator statusIndicator;
    public Text apCost;
    public Text playerName;
    public Text aiAction;
    private GameController gc;

    // Use this for initialization
    void Awake () {
        if (apCost == null)
            Debug.LogError("No AP Cost text object assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("No StatusIndicator assigned to " + gameObject.name);

        gc = GameObject.Find("GameController").GetComponent<GameController>();
        aiAction = transform.Find("AIAction").GetComponent<Text>(); ;
        gc.onUnitChange += LoadStats;
        gc.onUnitChange += ShowHealthBars;
        gc.onUnitChange += ToggleAIAction;
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

    public void ShowHealthBars(CharController currentCharacter)
    {
        foreach(GameObject charGO in gc.battleCharacters)
        {
            CharController character = charGO.GetComponent<CharController>();
            character.statusIndicator.gameObject.SetActive(true);
        }
        currentCharacter.statusIndicator.gameObject.SetActive(false);
    }

    public void LoadStats(CharController currentCharacter)
    {
        playerName.text = currentCharacter.Name;
        statusIndicator.SetHealth(currentCharacter.Stats.curHealth, currentCharacter.Stats.maxHealth);
        statusIndicator.SetAP(currentCharacter.Stats.curAP, currentCharacter.Stats.maxAP);
        statusIndicator.SetMP(currentCharacter.Stats.curMP, currentCharacter.Stats.maxMP);
    }

}
