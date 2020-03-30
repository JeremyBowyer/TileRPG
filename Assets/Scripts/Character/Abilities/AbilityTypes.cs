using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntentTypes {

    public enum Intent { Hostile, Support, Heal };
	
    public static Color GetIntentColor(Intent intent)
    {
        Color intentColor;
        if (intent == Intent.Hostile)
        {
            intentColor = CustomColors.AttackRange;
        }
        else if (intent == Intent.Heal)
        {
            intentColor = CustomColors.Heal;
        }
        else
        {
            intentColor = CustomColors.Support;
        }

        return intentColor;
    }

}
