using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController
{
    public List<CharController> roundChars;
    public BattleController bc;

    public RoundController(BattleController _bc)
    {
        bc = _bc;
    }

    public void InitRound(List<GameObject> characters)
    {
        roundChars = new List<CharController>();
        foreach(GameObject go in characters)
        {
            CharController controller = go.GetComponent<CharController>();
            if (controller != null)
                roundChars.Add(controller);
        }
        DetermineTurnOrder();
        if (bc.onRoundChange != null)
            bc.onRoundChange();
    }

    public void CharacterTurnEnd(CharController character)
    {
        roundChars.Remove(character);
    }

    public void DetermineTurnOrder()
    {
        bool sorted = false;
        while (!sorted)
        {
            bool swap = false;
            for (int i = 0; i < roundChars.Count - 1; i++)
            {
                CharController a = roundChars[i];
                CharController b = roundChars[i + 1];

                if (a.Stats.initiative < b.Stats.initiative)
                {
                    roundChars[i] = b;
                    roundChars[i + 1] = a;
                    swap = true;
                }
            }
            sorted = !swap;
        }
    }
}
