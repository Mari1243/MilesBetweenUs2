using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;


    public enum States
    {
        Gasstation,
        Car,
    }

public class NewJournalSave : MonoBehaviour
{
    // public static event System.Action<bool> ChangeJournal;

    public static NewJournalSave instance;
    public States currentstate;

    //to do list is going to be a different object depending on the scene, should prob assign by searching by name for it in the scene 
    //public GameObject[] ToDoListPrefabs;
    [SerializeField] private ToDoListData toDoListData;

    private GameObject currentList;
    private GameObject journal;
    private int sceneList = 0;

    public GameObject inventoryObject;



    //for spawning stuff in journal
    public GameObject DraggableItemPrefab;
    public GameObject tabholder;
    private GameObject Tab1;
    public List<InventoryItem> currentInventory = new List<InventoryItem>();

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        journal = this.transform.GetChild(0).gameObject;
        print("journal object name is "+journal.name);

        Tab1 = tabholder.transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        currentInventory = InventoryManager.instance.inventory;
    }

         private void OnEnable()
    {
        InventoryManager.AddedItem += CollectedLoreItem;

    }
    private void OnDisable()
    {
        InventoryManager.AddedItem -= CollectedLoreItem;

    }

    public void newspawnlist(GameObject data)
    {
        if (data != null)
        {
            currentList = Instantiate(data, journal.transform);
            currentList.transform.localPosition = new Vector3(-214, 65, 0);
            print("Spawned: " + currentList.name);
        }
        else
        {
         Debug.LogError($"Prefab at index {sceneList} is null or out of range!", this);
        }
    }
    private void CollectedLoreItem(Item item)
    {
        
    }

    // private void SpawnList()
    // {
    //     //this should be called by scenetrackersingleton, sending over the scene todoitem


    //     // Debug.Log($"toDoListData is null: {toDoListData == null}");
    //     // if (toDoListData != null)
    //     // {
    //     //     Debug.Log($"Array length: {toDoListData.ToDoListPrefabs.Length}");
    //     //     Debug.Log($"Element 0 is null: {toDoListData.ToDoListPrefabs[0] == null}");
    //     // }

    //     if (sceneList < toDoListData.ToDoListPrefabs.Length && toDoListData.ToDoListPrefabs[sceneList] != null)
    //     {
    //     currentList = Instantiate(toDoListData.ToDoListPrefabs[sceneList], journal.transform);
    //     currentList.transform.localPosition = new Vector3(-214, 65, 0);
    //     print("Spawned: " + currentList.name);
    //     }
    //     else
    //     {
    //     Debug.LogError($"Prefab at index {sceneList} is null or out of range!", this);
    //     }
       
    //     // if (sceneList < ToDoListPrefabs.Length && ToDoListPrefabs[sceneList] != null)
    //     // {
    //     //     print("trying to instantiate current list "+ ToDoListPrefabs[sceneList]);
    //     //     currentList = Instantiate(ToDoListPrefabs[sceneList], journal.transform);
    //     //     currentList.transform.localPosition = new Vector3(-214,65, 0);
    //     //     print("Spawned: " + currentList.name + " | Parent: " + currentList.transform.parent.name + " | Active: " + currentList.activeSelf);
    //     // }
    // }

    public void SetState(States newstate)
    {
        print("setting state");
        if (newstate == States.Gasstation)
        {
            GasStationJournal();
        }
        else if (newstate == States.Car)
        {
            CarJournal();
        }
    }
    private void GasStationJournal()
    {
        print("setting journal state to GAS STATION");
        
        //SpawnList();

        if (inventoryObject != null)
            inventoryObject.SetActive(false);
        if (currentList != null)
        {
            currentList.SetActive(true);
        }
        sceneList++; // advance to next list in sequence
    }
    private void CarJournal()
    {
       print("setting journal state to CAR");
        
        if (inventoryObject != null)
            inventoryObject.SetActive(true);
        if(currentList != null)
        {
            //may be causing errors???]
            Destroy(currentList);
        }

        StartCoroutine(manage());
    }

    private IEnumerator manage()
    {
        yield return new WaitForSeconds (1f);
        manageItems();
    }

    public void manageItems()
    {
        //Inventory
        if (currentInventory.Count == 0)
        {
            print("Empty");
        }
        else
        {
            print("instantiating inventory");
            foreach (InventoryItem items in currentInventory)
            {
                var journalItem = Instantiate(DraggableItemPrefab, Vector3.one, Quaternion.identity); //the connecting data

                //getting tab 1
                
                journalItem.transform.SetParent(Tab1.transform);
                journalItem.transform.localPosition = Vector2.zero;

                //this  assigns data
                DraggableItemPrefab.GetComponent<DragItem>().itemdata=items.itemData;
                print("instantiating " + journalItem.name);
            }
        }
    }
}
