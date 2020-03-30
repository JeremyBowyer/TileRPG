using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    List<Item> inventory;
    public Dictionary<Type, int> typeCount;
    public int capacity;
    private Currency currency;

    public int Count
    {
        get
        {
            if (inventory == null)
                return 0;
            return inventory.Count;
        }
    }
    public int weight
    {
        get
        {
            int total = 0;

            foreach(Item item in inventory)
            {
                total += item.iWeight;
            }

            return total;
        }
    }

    public int freeSpace
    {
        get
        {
            return capacity - weight;
        }
    }

    public Inventory()
    {
        inventory = new List<Item>();
        typeCount = new Dictionary<Type, int>();
        currency = new Currency(0);
        capacity = 100;
    }

    public Inventory(int _capacity)
    {
        inventory = new List<Item>();
        typeCount = new Dictionary<Type, int>();
        currency = new Currency(0);
        capacity = _capacity;
    }

    public Inventory(int _capacity, int _currencyAmount)
    {
        inventory = new List<Item>();
        typeCount = new Dictionary<Type, int>();
        currency = new Currency(_currencyAmount);
        capacity = _capacity;
    }

    public void SortBy(bool ascending = true)
    {
        inventory.Sort((x, y) => x.iWeight.CompareTo(y.iWeight));
    }

    public void AddCurrency(int _amt)
    {
        currency.Add(_amt);
    }

    public void AddCurrency(Currency currency)
    {
        currency.Add(currency.amount);
    }

    public bool RemoveCurrency(int amt)
    {
        if (amt > currency.amount)
            return false;

        currency.amount -= amt;
        return true;
    }

    public bool RemoveCurrency(Currency currency)
    {
        return RemoveCurrency(currency.amount);
    }

    public bool Add(Item item)
    {
        if (item is Currency)
        {
            AddCurrency(item as Currency);
            return true;
        }

        if (item.iWeight > freeSpace)
            return false;

        inventory.Add(item);

        if (typeCount.ContainsKey(item.GetType()))
        {
            typeCount[item.GetType()]++;
        }
        else
        {
            typeCount[item.GetType()] = 1;
        }

        return true;
    }

    public bool Add(List<Item> items)
    {
        foreach(Item item in items)
        {
            if (!Add(item))
                return false;
        }

        return true;
    }

    public string GetTypeName(Type type)
    {
        Item item = (Item)Activator.CreateInstance(type);
        if (item != null)
            return item.iName;

        return null;
    }

    public string GetTypeDescription(Type type)
    {
        Item item = (Item)Activator.CreateInstance(type);
        if (item != null)
            return item.iDescription;

        return null;
    }

    public void Remove(Item item)
    {
        if (item is Currency)
            RemoveCurrency(item as Currency);

        inventory.Remove(item);
        typeCount[item.GetType()]--;
    }

    public void RemoveAll()
    {
        List<Item> inventoryCopy = new List<Item>(inventory);
        foreach (Item item in inventoryCopy)
        {
            Remove(item);
        }
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

    public int GetItemCount(Item item)
    {
        if (item is Currency)
            return currency.amount;

        if (typeCount.ContainsKey(item.GetType()))
            return typeCount[item.GetType()];
        else
            return 0;
    }

    public List<Item> GetItemsList()
    {
        return inventory;
    }

    public IEnumerable GetItems()
    {
        foreach(Item item in inventory)
        {
            yield return item;
        }
    }

    public int GetCurrencyAmount()
    {
        return currency.amount;
    }

    public Currency GetCurrency()
    {
        return currency;
    }

}
