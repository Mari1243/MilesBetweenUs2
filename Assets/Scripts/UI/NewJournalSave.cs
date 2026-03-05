using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NewJournalSave : MonoBehaviour
{
    // public static event System.Action<bool> ChangeJournal;

    public static NewJournalSave instance;
    public States currentstate;

    //to do list is going to be a different object depending on the scene, should prob assign by searching by name for it in the scene 
    public GameObject[] ToDoListPrefabs;
    [SerializeField] private GameObject currentList;
    private int sceneList = 0;

    public GameObject inventoryObject;

    public enum States
    {
        Gasstation,
        Car,
    }


    // void OnEnable()
    // {
    //     NewJournalSave.ChangeJournal += OnJournalChanged;
    // }

    // void OnDisable()
    // {
    //     NewJournalSave.ChangeJournal -= OnJournalChanged;
    // }


    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SpawnList()
    {
        if (currentList != null)
            Destroy(currentList);
            print("destroying currentList");

        if (sceneList < ToDoListPrefabs.Length && ToDoListPrefabs[sceneList] != null)
        {
            currentList = Instantiate(ToDoListPrefabs[sceneList], this.transform);
            currentList.transform.localPosition = new Vector3(-214,65, 0);
            currentList.transform.localScale = new Vector3(.2f,.2f, 1);
            print("Spawned: " + currentList.name + " | Parent: " + currentList.transform.parent.name + " | Active: " + currentList.activeSelf);
        }
    }

    public void SetState(States newstate)
    {
        print("setting state");
        if (newstate == NewJournalSave.States.Gasstation)
        {
            GasStationJournal();
        }
        else if (newstate == NewJournalSave.States.Car)
        {
            CarJournal();
        }
    }
    private void GasStationJournal()
    {
        print("setting journal state to GAS STATION");
        
        SpawnList();

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
        
        if (currentList != null)
        {
            Destroy(currentList);
            currentList = null;
        }
        if (inventoryObject != null)
            inventoryObject.SetActive(true);
    }
}
