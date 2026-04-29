using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public int numberofSlots;
    public List<ItemSlot> inventorySlots;
    private void OnEnable()
    {
        // InventoryManager.OnInventoryChange += DrawInventory;
        InventoryManager.StoreItem += storeItem;
    }
    private void OnDisable()
    {
        // InventoryManager.OnInventoryChange -= DrawInventory;
        InventoryManager.StoreItem -= storeItem;        
    }
    private void Start()
    {
        inventorySlots = new List<ItemSlot>(numberofSlots);
    }
    
    private void storeItem(Item item)
    {
        print("calling store item");
        //add slot with new item image
        if (inventorySlots.Count < numberofSlots)
        {
            print("creating inventory slot");
            CreateInventorySlot(item);
        }
        else
        {
            print("inventory full, cant add to journal");
        }
    }
    void ResetInventory()
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
        inventorySlots = new List<ItemSlot>(numberofSlots);
    }

    // void DrawInventory(List<InventoryItem> inventory)
    // {
    //     for (int i = 0; i < inventory.Count; i++)
    //     {
    //         inventorySlots[i].DrawSlot(inventory[i]);
    //     }
    // }


    void CreateInventorySlot(Item item)
    {
        print("adding "+ item.name);
        numberofSlots++;
        GameObject newSlot = Instantiate(slotPrefab, this.transform);
        ItemSlot newSlotComponent = newSlot.GetComponent<ItemSlot>();

        newSlotComponent.DrawSlot(item);
        //newSlotComponent.ClearSlot();
        inventorySlots.Add(newSlotComponent);

    }
}
