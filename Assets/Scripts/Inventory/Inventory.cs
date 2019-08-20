using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    List<Item> inventory;
    public Dictionary<Type, int> typeCount;

    public Inventory()
    {
        inventory = new List<Item>();
        typeCount = new Dictionary<Type, int>();
    }

    public void Add(Item item)
    {
        inventory.Add(item);

        if (typeCount.ContainsKey(item.GetType()))
        {
            typeCount[item.GetType()]++;
        }
        else
        {
            typeCount[item.GetType()] = 1;
        }
    }

    public void Remove(Item item)
    {
        inventory.Remove(item);
        typeCount[item.GetType()]--;
    }

    public bool Contains(Item item)
    {
        return inventory.Contains(item);
    }

    public bool ContainsType(Type type)
    {
        return typeCount.ContainsKey(type) && typeCount[type] > 0;
    }

    public Item GetItemOfType(Type type)
    {
        foreach(Item item in inventory)
        {
            if(item.GetType() == type)
            {
                Remove(item);
                return item;
            }
        }

        return null;
    }

}
