using DG.Tweening;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static event Action<List<InventoryItem>> OnInventoryChange;
    public static InventoryManager instance;
    //Inventory Lists

    public List<InventoryItem> inventory = new List<InventoryItem>();
    public Dictionary<Item, InventoryItem> itemDictionary = new Dictionary<Item, InventoryItem>();
    public int loreItemAmt;
    public float moneyAmount;

    private void OnEnable()
    {
        interactable.onPickedUp += Add;
        Mapinteractable.onPickedUp += Add;
  
    }
    private void OnDisable()
    {
        interactable.onPickedUp -= Add;
        Mapinteractable.onPickedUp -= Add;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Update()
    {
    }
    public void Add(Item itemData) 
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item)) //is item in dictionary
        {
            item.AddtoStack();
            OnInventoryChange?.Invoke(inventory);
        }
        else //creates new inventory item and stores it into list
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            itemDictionary.Add(itemData, newItem);

            OnInventoryChange?.Invoke(inventory);
        }
    }

    public void RemoveItem(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventoryItem item)) 
        {
            item.RemoveStack();
            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }
            OnInventoryChange?.Invoke(inventory);
        }

    }

    public bool HasItem(string itemName)
    {
        print("checking for " + itemName + " item");
        return inventory.Any(item => item.itemData.itemName == itemName);
    }

 

}
