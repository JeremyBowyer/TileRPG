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

    public void ToggleAIAction(Character currentCharacter)
    {
        aiAction.gameObject.SetActive(currentCharacter is Enemy);
    }

    public void ShowHealthBars(Character currentCharacter)
    {
        foreach(GameObject charGO in gc.battleCharacters)
        {
            Character character = charGO.GetComponent<Character>();
            character.statusIndicator.gameObject.SetActive(true);
        }
        currentCharacter.statusIndicator.gameObject.SetActive(false);
    }

    public void LoadStats(Character currentCharacter)
    {
        playerName.text = currentCharacter.characterName;
        statusIndicator.SetHealth(currentCharacter.stats.curHealth, currentCharacter.stats.maxHealth);
        statusIndicator.SetAP(currentCharacter.stats.curAP, currentCharacter.stats.maxAP);
        statusIndicator.SetMP(currentCharacter.stats.curMP, currentCharacter.stats.maxMP);
    }

}
