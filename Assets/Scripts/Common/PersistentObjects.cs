using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjects
{
    // Protag and co.
    public static PartyMember protagonist = new Executioner();
    public static Party party = new Party();
    public static Roster roster = new Roster();
    public static Inventory bag = new Inventory();
    public static Inventory vault = new Inventory() { capacity = 999 };
    public static Vector3 protagonistLocation;

    // Generic
    public static List<string> deadObjects;

    public static void SaveProtagonist(ProtagonistController protag)
    {
        protagonist = protag.character as PartyMember;
        bag = protag.inventory;
    }
}
