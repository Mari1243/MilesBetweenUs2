using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject slotPrefab;
    [SerializeField] private int numberofSlots;

    public int MAXslots;
    public GameObject[] parents;

    public List<ItemSlot> inventorySlots;


    private GameObject currentparent; 
    private int parentnum = 0;


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
    inventorySlots = new List<ItemSlot>();
    currentparent = parents[parentnum];
    numberofSlots = 0; // Tracks slots used in CURRENT parent
}

private void storeItem(Item item)
{
    print("calling store item");

    if (numberofSlots >= MAXslots)
    {
        if (parentnum < parents.Length - 1)
        {
            parentnum++;
            numberofSlots = 0;
            inventorySlots = new List<ItemSlot>();
            currentparent = parents[parentnum];
            print("moving to next parent: " + currentparent.gameObject.name);
        }
        else
        {
            print("all parents full, cant add to journal");
            return;
        }
    }

    CreateInventorySlot(item);
}   

    void ResetInventory()
    {
        foreach (Transform childTransform in currentparent.transform)
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
        numberofSlots++;
        GameObject newSlot = Instantiate(slotPrefab, currentparent.transform);
        ItemSlot newSlotComponent = newSlot.GetComponent<ItemSlot>();

        newSlotComponent.DrawSlot(item);
        //newSlotComponent.ClearSlot();
        inventorySlots.Add(newSlotComponent);
        print("INSTANTIATING child " + item + "under " + currentparent.name);
    }
}
