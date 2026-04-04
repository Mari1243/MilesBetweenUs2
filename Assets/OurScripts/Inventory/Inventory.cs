using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{



    public GameObject slotPrefab;
    public int numberofSlots;
    public List<ItemSlot> inventorySlots;
    private void OnEnable()
    {
        
        InventoryManager.OnInventoryChange += DrawInventory;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= DrawInventory;
    }
    private void Start()
    {
           inventorySlots = new List<ItemSlot>(numberofSlots);

}
    void ResetInventory()
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
        inventorySlots = new List<ItemSlot>(numberofSlots);
    }

    void DrawInventory(List<InventoryItem> inventory)
    {
        ResetInventory();

        for (int i = 0; i < inventorySlots.Capacity; i++)
        {
            CreateInventorySlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].DrawSlot(inventory[i]);
        }
    }


    void CreateInventorySlot()
    {
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(transform, false);

        ItemSlot newSlotComponent = newSlot.GetComponent<ItemSlot>();
        newSlotComponent.ClearSlot();

        inventorySlots.Add(newSlotComponent);

    }
}
