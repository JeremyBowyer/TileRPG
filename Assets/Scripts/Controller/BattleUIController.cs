using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour {

    public GlobalStatusIndicator currentStatusIndicator;
    public GlobalStatusIndicator targetStatusIndicator;
    public Text apCost;
    public Text playerName;
    public Text targetName;
    public Text aiAction;
    private BattleController bc;


    // Use this for initialization
    void Awake () {
        if (apCost == null)
            Debug.LogError("No AP Cost text object assigned to " + gameObject.name);

        if (currentStatusIndicator == null)
            Debug.LogError("No StatusIndicator assigned to " + gameObject.name);

        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        bc.onUnitChange += LoadCurrentStats;
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
            if (charGO.activeSelf)
            {
                CharController character = charGO.GetComponent<CharController>();
                character.statusIndicator.gameObject.SetActive(true);
                character.statusIndicator.SetCurrentHP(character.Stats.curHP, character.Stats.maxHPTemp, character.Stats.maxHP);
            }
        }
    }

    public void UpdateCurrentStats(bool animate = true)
    {
        if (bc.CurrentCharacter != null)
        {
            playerName.text = bc.CurrentCharacter.Name;
            currentStatusIndicator.SetCurrentHP(bc.CurrentCharacter.Stats.curHP, bc.CurrentCharacter.Stats.maxHPTemp, bc.CurrentCharacter.Stats.maxHP, true);
            currentStatusIndicator.SetCurrentAP(bc.CurrentCharacter.Stats.curAP, bc.CurrentCharacter.Stats.maxAPTemp, bc.CurrentCharacter.Stats.maxAP, true);
            currentStatusIndicator.SetCurrentMP(bc.CurrentCharacter.Stats.curMP, bc.CurrentCharacter.Stats.maxMPTemp, bc.CurrentCharacter.Stats.maxMP, true);
            currentStatusIndicator.AddEffects(bc.CurrentCharacter.maladies);
        }
    }

    public void LoadCurrentStats(CharController currentCharacter)
    {
        if (currentCharacter != null)
        {
            playerName.text = currentCharacter.Name;
            currentStatusIndicator.SetCurrentHP(currentCharacter.Stats.curHP, currentCharacter.Stats.maxHPTemp, currentCharacter.Stats.maxHP, false);
            currentStatusIndicator.SetCurrentAP(currentCharacter.Stats.curAP, currentCharacter.Stats.maxAPTemp, currentCharacter.Stats.maxAP, false);
            currentStatusIndicator.SetCurrentMP(currentCharacter.Stats.curMP, currentCharacter.Stats.maxMPTemp, currentCharacter.Stats.maxMP, false);
            currentStatusIndicator.AddEffects(currentCharacter.maladies);
        }
    }

    public void UpdateTargetStats(bool animate = true)
    {
        if (bc.TargetCharacter != null)
        {
            targetName.text = bc.TargetCharacter.Name;
            targetStatusIndicator.SetCurrentHP(bc.TargetCharacter.Stats.curHP, bc.TargetCharacter.Stats.maxHPTemp, bc.TargetCharacter.Stats.maxHP, true);
            targetStatusIndicator.SetCurrentAP(bc.TargetCharacter.Stats.curAP, bc.TargetCharacter.Stats.maxAPTemp, bc.TargetCharacter.Stats.maxAP, true);
            targetStatusIndicator.SetCurrentMP(bc.TargetCharacter.Stats.curMP, bc.TargetCharacter.Stats.maxMPTemp, bc.TargetCharacter.Stats.maxMP, true);
            targetStatusIndicator.AddEffects(bc.TargetCharacter.maladies);
        }
    }

    public void LoadTargetStats(CharController targetCharacter)
    {
        targetStatusIndicator.transform.parent.gameObject.SetActive(true);
        if (targetCharacter != null)
        {
            targetName.text = targetCharacter.Name;
            targetStatusIndicator.SetCurrentHP(targetCharacter.Stats.curHP, targetCharacter.Stats.maxHPTemp, targetCharacter.Stats.maxHP, false);
            targetStatusIndicator.SetCurrentAP(targetCharacter.Stats.curAP, targetCharacter.Stats.maxAPTemp, targetCharacter.Stats.maxAP, false);
            targetStatusIndicator.SetCurrentMP(targetCharacter.Stats.curMP, targetCharacter.Stats.maxMPTemp, targetCharacter.Stats.maxMP, false);
            targetStatusIndicator.AddEffects(targetCharacter.maladies);
        }
    }

    public void UnloadTargetStats()
    {
        targetStatusIndicator.transform.parent.gameObject.SetActive(false);
    }

}
