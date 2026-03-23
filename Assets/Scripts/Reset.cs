using System.ComponentModel;
using UnityEngine;

public class Reset : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("calling refresh");
        GameObject journal = GameObject.Find("journalCanvas");
        if(journal != null)
        {
            journal.GetComponent<NewJournalSave>().currentInventory.Clear();
            //then i need to also kill gameobjects
            Destroy(journal);
        }

        GameObject inventorymanager = GameObject.Find("InventoryManager");
        if(inventorymanager != null)
        {
            inventorymanager.GetComponent<InventoryManager>().inventory.Clear();
            Destroy(inventorymanager);
        }
        // GameObject j = GameObject.Find("TransitionManager");
        // if(k != null)
        // {
        //     Destroy(journal);
        // }
        // GameObject h = GameObject.Find("SceneTracker");
        // if(h != null)
        // {
        //     Destroy(journal);
        // }
    }

}
