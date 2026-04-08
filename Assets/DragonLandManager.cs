using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonLandManager : MonoBehaviour
{
    public Item startCutScene;
    public GameObject bro;
    public Animator car;

    

    //for to do list
    [SerializeField]private bool completedAllObjectives;
    public int allobjectives = 1;
    private int completedobjectives = 0;

    //to trigger popup
    public static event Action journalNotif;

    private void Start()
    {
        bro.SetActive(false);
        car.Play("DLCar");
    }

     private void OnEnable()
    {
        InventoryManager.OnInventoryChange += checkconditions;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= checkconditions;
    }

    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void checkconditions(List<InventoryItem> list)
    {
        if (!completedAllObjectives)
        {
            foreach (InventoryItem item in list)
            {
                //specific quest
                if (item.itemData.itemName == "JacketPatch")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                   ToDoManager.instance.CompleteItem("SouvenirforMax");
                    completedobjectives++;
                }
                if (completedobjectives >= allobjectives)
                {
                    completedAllObjectives = true;
                    print("completed all level objectives yay");
                }
            }
        }
    }
    //not totally sure how this works
    // public void StartAction(string action)
    // {

    //     switch (action)
    //     {
    //         case "StartbroQuest":
    //             journalNotif?.Invoke();

    //             break;


    //     }
    // }
    

}
