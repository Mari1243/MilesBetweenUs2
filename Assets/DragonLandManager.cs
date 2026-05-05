using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonLandManager : MonoBehaviour
{
    public Item startCutScene;
    public GameObject bro;
    public Animator car;
    public Item MouseReward;

    public bool canleave = false;

    private bool completedMouseQuest = false;
    public GameObject player;


    //for to do list
    [SerializeField]private bool completedAllObjectives;
    public int allobjectives = 1;
    private int completedobjectives = 0;

    //to trigger popup
    public static event Action journalNotif;

    private void Start()
    {
        bro.SetActive(false);
        player.SetActive(false);
        car.Play("DLCar");
    }

     private void OnEnable()
    {
        InventoryManager.OnInventoryChange += checkconditions;
        DialogueCommands.startAction += StartAction;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= checkconditions;
        DialogueCommands.startAction -= StartAction;    
    }

    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        player.SetActive(true);

        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void checkconditions(List<InventoryItem> list)
    {
            foreach (InventoryItem item in list)
            {
                //specific quest
                if (item.itemData.itemName == "Dragon Plushie")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                   ToDoManager.instance.CompleteItem("SouvenirforMax");
                   canleave = true;
                   //allow option to leave here
                    completedobjectives++;
                }
                if (item.itemData.itemName == "An ID!")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                    ToDoManager.instance.CompleteItem("MouseQuest");
                    completedobjectives++;
                }
                if (completedobjectives >= allobjectives)
                {
                    completedAllObjectives = true;
                    print("completed all level objectives yay");
                }
            }

    }
    public void StartAction(string action)
    {

        switch (action)
        {
            case "mouseQuest":
           
                if (!completedMouseQuest)
                {
                    InventoryManager.instance.Add(MouseReward);
         
                    completedMouseQuest = true;

                    //check off number
                    ToDoManager.instance.CompleteItem("MouseQuest");
                }
               
                break;

            case "StartmouseQuest":

                journalNotif?.Invoke();
                ToDoManager.instance.spawnnewToDoTask("MouseQuest", "Find Mouse girl's ID!");
                break;

        }
    }
}



