using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjects
{
    // Protag and co.
    public static Character protagonist;
    public static List<PartyMember> partyMembers;
    public static Inventory inventory;
    public static Vector3 protagonistLocation;

    // Enemies
    public static string enemyName;
    public static Enemy battleInitiator;

    // Generic
    public static List<string> deadObjects;

    public static void SaveProtagonist(GameController gc)
    {
        protagonist = gc.protag.character;
        partyMembers = gc.protag.partyMembers;
        inventory = gc.protag.inventory;
    }

    public static void RemoveObject(string id)
    {
        if (deadObjects == null)
        {
            deadObjects = new List<string>();
        }

        deadObjects.Add(id);
    }

}
