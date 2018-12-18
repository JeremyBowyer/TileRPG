using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankItem : Item
{
    public BlankItem()
    {
        throw new System.Exception("Tried to get item type that doesn't exist in inventory.");
    }
}
