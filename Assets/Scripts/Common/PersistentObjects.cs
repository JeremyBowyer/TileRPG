using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjects
{
    // Protag and co.
    public static Character protagonist;
    public static List<PartyMember> partyMembers;
    public static Inventory inventory;

    // Enemies
    public static List<KeyValuePair<Enemy, Vector3>> enemies;

    public static void SaveProtagonist(GameController gc)
    {
        protagonist = gc.protag.character;
        partyMembers = gc.protag.partyMembers;
        inventory = gc.protag.inventory;
    }

}
