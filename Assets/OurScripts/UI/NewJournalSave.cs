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
            Debug.LogError("[NJS] Awake fired | instance ID: " + gameObject.GetInstanceID());

            if (instance != null && instance != this)
            {
                Debug.LogError("[NJS] DUPLICATE FOUND, destroying: " + gameObject.GetInstanceID()
                    + " keeping: " + instance.gameObject.GetInstanceID());
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            journal = this.transform.GetChild(0).gameObject;
        //print("journal object name is "+journal.name);

        Tab1 = tabholder.transform.GetChild(0).gameObject;

        // Add this in Awake() after getting the journal reference
        xbutton.onClick.AddListener(() =>
     Debug.LogError("[NJS] XBUTTON PHYSICALLY CLICKED | instance ID: "
         + xbutton.gameObject.GetInstanceID())); ;
    }

    private void Start()
    {
        currentInventory = InventoryManager.instance.inventory;
    }

    private void OnEnable()
    {
        InventoryManager.AddedItem += CollectedLoreItem;
        DialogueCommands.EndJournalState +=EndJournal;
        //added check
        //Debug.LogError("[NJS] OnEnable: subscribed to EndJournalState.");
    }
    private void OnDisable()
    {
        InventoryManager.AddedItem -= CollectedLoreItem;
         DialogueCommands.EndJournalState -=EndJournal;
         //added check
         // Debug.LogError("[NJS] OnDisable: UNSUBSCRIBED from EndJournalState. If this fires at wrong time, journal won't respond.");
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
         //Debug.LogError($"Prefab at index {sceneList} is null or out of range!", this);
        }
    }
    private void CollectedLoreItem(Item item)
    {
        
    }

    public void SetState(States newstate)
    {
        Debug.LogError("[NJS] SetState called with: " + newstate + " | StackTrace: " + System.Environment.StackTrace);
        // ... rest of method
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
        SetXButtonListener(ToggleJournalSafe); // match CarJournal
        Debug.LogError("XBUTTON NOW CLOSES JOURNAL");

        if (inventoryObject != null)
            inventoryObject.SetActive(false);
        if (currentList != null)
            currentList.SetActive(true);
        sceneList++;
    }
    //added checks
    private void EndJournalState() 
    {
        xbutton.gameObject.SetActive(true);
        UnityAction action = () => TriggerEndDialogue();
        SetXButtonListener(action);
        Debug.LogError("[NJS] XBUTTON NOW listens set to TriggerEndDialogue.");


        if (inventoryObject != null)
        inventoryObject.SetActive(true);
    if (currentList != null)
    {
        //Debug.LogError("[NJS] Destroying currentList: " + currentList.name);
        Destroy(currentList);
    }

    //Debug.LogError("[NJS] Starting manage() coroutine.");
    StartCoroutine(manage());
    }

    //added checks
    private void TriggerEndDialogue()
    {
        Debug.LogError("[NJS] TriggerEndDialogue FIRED.");
        if (xbutton == null) Debug.LogError("[NJS] xbutton null at fire time!");
        if (DialogueManager.instance == null) Debug.LogError("[NJS] DialogueManager.instance NULL!");

        Item item = xbutton.GetComponent<interactable>()?.item;
        if (item == null) Debug.LogError("[NJS] item on xbutton interactable is NULL!");

        DialogueManager.instance.TalkInteraction(item);
    }
    //added checks
    private void EndJournal(bool isEndstate)
    {
    //Debug.LogError($"[NJS] EndJournal received. isEndstate={isEndstate}");
    if (isEndstate)
    {
        //Debug.LogError("[NJS] Calling SetState(States.End)");
        SetState(States.End);
    }
    else
    {
        //Debug.LogError("[NJS] Calling SetState(States.Gasstation)");
        SetState(States.Gasstation);
    }
    }

    private void CarJournal()
    {
        print("setting journal state to CAR"); // fix this
        SetXButtonListener(ToggleJournalSafe);
        Debug.LogError("[NJS] XBUTTON NOW listens set to CARRRR."); // fix this

        if (inventoryObject != null)
            inventoryObject.SetActive(true);
        if (currentList != null)
            Destroy(currentList);

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

    private void SetXButtonListener(UnityAction action)
    {
        if (xbutton == null) { Debug.LogError("[NJS] xbutton NULL!"); return; }

        xbutton.onClick.RemoveAllListeners();
        xbutton.onClick.AddListener(action);
        xbutton.interactable = true;
        Debug.LogError("[NJS] xbutton listener set to: " + action.Method.Name
            + " | interactable: " + xbutton.interactable
            + " | gameObject active: " + xbutton.gameObject.activeInHierarchy
            + "\n" + System.Environment.StackTrace);  // ADD STACK TRACE
    }

    private void ToggleJournalSafe()
    {
        ToggleJournal tj = GetComponent<ToggleJournal>();
        if (tj == null)
        {
            Debug.LogError("[NJS] ToggleJournal component not found on this GameObject!");
            return;
        }
        tj.journal();
    }
}
