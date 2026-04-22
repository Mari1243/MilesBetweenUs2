using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;


public enum States
    {
        Gasstation,
        Car,
        End,
    }

public class NewJournalSave : MonoBehaviour
{
    // public static event System.Action<bool> ChangeJournal;

    public static NewJournalSave instance;
    public States currentstate;

    public GameObject instatiateTarget; 

    //to do list is going to be a different object depending on the scene, should prob assign by searching by name for it in the scene 
    //public GameObject[] ToDoListPrefabs;
    [SerializeField] private ToDoListData toDoListData;

    private GameObject currentList;
    private GameObject journal;
    private int sceneList = 0;

    public GameObject inventoryObject;
    public Button xbutton;



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
        DialogueCommands.EndJournalState +=EndJournal;
        
    }
    private void OnDisable()
    {
        InventoryManager.AddedItem -= CollectedLoreItem;
         DialogueCommands.EndJournalState -=EndJournal;
    }

    public void newspawnlist(GameObject data)
    {
        if (data != null)
        {
            currentList = Instantiate(data, journal.transform);
            Vector3 vec = new Vector3(instatiateTarget.transform.position.x,instatiateTarget.transform.position.y,0);
            currentList.transform.position = vec;
            //print("Spawned: " + currentList.name);
        }
        else
        {
         Debug.LogError($"Prefab at index {sceneList} is null or out of range!", this);
        }
    }
    private void CollectedLoreItem(Item item)
    {
        
    }

    public void SetState(States newstate)
    {
        print("setting state");
        print("new state is " + newstate);
        if (newstate == States.Gasstation)
        {
            GasStationJournal();
        }
        else if (newstate == States.Car)
        {
            CarJournal();
        }
        else if (newstate == States.End)
        {
            EndJournalState();
        }
    }
    private void GasStationJournal()
    {
        print("setting journal state to GAS STATION");
        
        //set the xbutton click to activate end dialogue
        xbutton.onClick.RemoveAllListeners();
        xbutton.onClick.AddListener(ToggleJournal.instance.journal);

        if (inventoryObject != null)
            inventoryObject.SetActive(false);
        if (currentList != null)
        {
            currentList.SetActive(true);
        }
        sceneList++; // advance to next list in sequence
        
    }

    private void EndJournalState()
    {
        //set the xbutton click to activate end dialogue
        print("triggering end journal state");
        xbutton.gameObject.SetActive(true);
        xbutton.onClick.RemoveAllListeners();
        xbutton.onClick.AddListener(TriggerEndDialogue);
         if (inventoryObject != null)
            inventoryObject.SetActive(true);
        if(currentList != null)
        {
            //may be causing errors???]
            Destroy(currentList);
        }

        StartCoroutine(manage());
    }
    

    private void TriggerEndDialogue()
    {
        DialogueManager.tutorialInstance.LoadDialog("ShowBrotherPrompt");
        DialogueManager.tutorialInstance.StartDialog();
    }

    private void EndJournal(bool isEndstate)
    {
        if (isEndstate)
        {
            print("activating end journal state");
            SetState(States.End);
        }
        else
        {
            print("activating normal journal state");
            SetState(States.Gasstation);
        }
        
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
            //print("instantiating inventory");
            foreach (InventoryItem items in currentInventory)
            {
                var journalItem = Instantiate(DraggableItemPrefab, Vector3.one, Quaternion.identity); //the connecting data

                //getting tab 1
                
                journalItem.transform.SetParent(Tab1.transform);
                journalItem.transform.localPosition = Vector2.zero;

                //this  assigns data
                DraggableItemPrefab.GetComponent<DragItem>().itemdata=items.itemData;
                //print("instantiating " + journalItem.name);
            }
            //added bc reinstantiation
            currentInventory.Clear();
        }
    }
}
