using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

[Serializable]
public class InventoryItem 
{
    public Item itemData;
    public int stackSize;
    public InventoryItem(Item ItemData)
    {
        itemData = ItemData;
    }

    public void AddtoStack()
    {
        stackSize++; 
    }
    public void RemoveStack()
    {
        stackSize--;
    }
}
