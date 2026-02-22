using System.Collections.Generic;
using UnityEngine;

public class ToDoManager : MonoBehaviour
{
    public static ToDoManager instance;
    public GameObject[] toDoItems;
    private Dictionary<string, ToDoItemBehavior> Tasks;

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

}
