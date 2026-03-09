using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToDoManager : MonoBehaviour
{
    public static ToDoManager instance;

    public GameObject[] toDoItems;
    private Dictionary<string, ToDoItemBehavior> Tasks;
    public GameObject[] Collectables;

    private void Awake()
    {
        instance = this;
        Tasks = new Dictionary<string, ToDoItemBehavior>();

        //this is gonna go through and add the script with the items name for easy access later
        foreach (var item in toDoItems)
        {
            var behavior = item.GetComponent<ToDoItemBehavior>();
            if (behavior != null)
                Tasks.Add(item.name, behavior);
        }
    }
    private void OnEnable()
    {
        InventoryManager.AddedItem += CollectedLoreItem;

    }
    private void OnDisable()
    {
        InventoryManager.AddedItem -= CollectedLoreItem;

    }

    public void CompleteItem(string itemName)
    {
        if (Tasks.TryGetValue(itemName, out ToDoItemBehavior item))
            item.SetState(ToDoItemState.Completed);
        else
            Debug.LogWarning($"ToDoItem '{itemName}' not found.");
    }

    public void ResetItem(string itemName)
    {
        if (Tasks.TryGetValue(itemName, out ToDoItemBehavior item))
            item.SetState(ToDoItemState.Incomplete);
        else
            Debug.LogWarning($"ToDoItem '{itemName}' not found.");
    }

    public void CollectedLoreItem(Item item)
    {
        foreach (var col in Collectables)
        {
            print(col.name);
            print(item.itemName);
            if (col.name == item.itemName)
            {
                TextMeshProUGUI txt = col.GetComponentInChildren<TextMeshProUGUI>();
                txt.text = item.itemName;
                col.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

}
