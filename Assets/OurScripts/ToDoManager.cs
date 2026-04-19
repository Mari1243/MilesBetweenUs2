using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ToDoManager : MonoBehaviour
{
    public static ToDoManager instance;
    public List<GameObject> toDoItems = new List<GameObject>();
    private Dictionary<string, ToDoItemBehavior> Tasks = new Dictionary<string, ToDoItemBehavior>();
    public GameObject[] Collectables;
    public GameObject taskprefab;
    public GameObject ToDoItemParent;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        //DontDestroyOnLoad(gameObject);
        //populating list with 
        foreach (var item in toDoItems)
        {
            var behavior = item.GetComponent<ToDoItemBehavior>();
            if (behavior != null)
            Tasks.Add(item.name, behavior);
        }
    }  

    private void addtasks(GameObject newobj)
    {
        var behavior = newobj.GetComponent<ToDoItemBehavior>();
        if (behavior != null)
        {
            Tasks.Add(newobj.name, behavior);
        }
        else
        {
            Debug.LogError("behavior is null on the task im adding");
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

    public void spawnnewToDoTask(string taskname, string taskdescription)
    {
        if (!Tasks.ContainsKey(taskname))
        {
            GameObject newTask = Instantiate(taskprefab, ToDoItemParent.transform);
            newTask.GetComponent<ToDoItemBehavior>().SetNewTaskInfo(taskname, taskdescription);
            addtasks(newTask);
        }  
    }

    public void CompleteItem(string itemName)
    {
        print("trying to check off item by the name of "+ itemName);
        if (Tasks.TryGetValue(itemName, out ToDoItemBehavior item))
        {
            item.SetState(ToDoItemState.Completed);
            print("successfully completed item");
        } 
        else
            Debug.LogWarning($"ToDoItem '{itemName}' not found.");

        foreach (KeyValuePair<string, ToDoItemBehavior> thing in Tasks)
        {
            print(thing.Key);
        }
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
        //added april 2nd
        if(Collectables.Length != 0)
        {
            print("this to do manager is activated and detected on "+ this.gameObject.name);
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

}
