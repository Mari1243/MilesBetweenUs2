using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


/*
 * This script basically just makes sure that the starting cutscene plays of the brother asking the player to get snacks, this can also where we code the objective being set up since the scripts we're going by are scene specific
 *as an additional functionality, this script that will be custom per scene is also going to keep track of what special items are in this scene
 */
public class GasStationManager : MonoBehaviour
{
    public Item item;
 public Item knife;
    public Animator car;
 //this exists so once ive completed all objectives it wont keep checking everytime i interact
 private static bool completedAllObjectives;
 private static int allobjectives = 2;
 private static int completedobjectives = 0;
 //public GameObject toDoList1;
 public static event Action journalNotif;
    private bool completedKidQuest = false;
 private bool firstTimeSteal = true;

//for tutorial
public GameObject TutorialDialogueSystem;
public GameObject DialogueSystem;
public DialogueRunner diaRun;
private bool hasExecuted = false;
 

 [SerializeField] private GameObject kidObjective;

    public void triggerIntroCutscene()
    {
        DialogueManager.instance.TalkInteraction(item);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChange += checkconditions;
        DialogueCommands.startAction += StartAction;
        Interactor.StealStep += StealTutorial;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= checkconditions;
        DialogueCommands.startAction -= StartAction;
        Interactor.StealStep -= StealTutorial;
    }

    private void StealTutorial(int stage)
    {
        
        //instructionss.SetActive(false);
        if (firstTimeSteal)
        {
            if (stage == 1)
            {
                StartCoroutine(instructions("instruction1"));
            }
            else if(stage == 2)
            {
                //be sure to only do this once!
                if (!hasExecuted)
                {
                    StartCoroutine(instructions("instruction2"));
                }
                hasExecuted = true;
            }
            else if(stage == 3)
            {
                //got caught
                StartCoroutine(instructions("gotCaught"));
            }
            else if(stage == 4)
            {
                StartCoroutine(instructions("ranOut"));
            }
            else if(stage == 5)
            {
                StartCoroutine(instructions("successfullyStole"));
                firstTimeSteal = false;
            }
          
        }
        return;
    }

    private IEnumerator instructions(string instruction)
    {
        if (DialogueManager.DialogStart != null)
        {
            diaRun.Stop();
        }
        yield return new WaitForSeconds(.3f);
        DialogueManager.tutorialInstance.LoadDialog(instruction);
        DialogueManager.tutorialInstance.StartDialog();
    }



    //conditions for tasks specifically in this level, this will involve checking each time you interact with something whether the conditions were met
    //1. collected snacks
    //2. got some sort of lore item for the brother
    //this is largely based on what is IN the inventory, its easier to check that way
    private void Start()
    {
        car.Play("GSCar");

        //GameObject mainPage = toDoList1.transform.GetChild(1).gameObject;
        //kidObjective = mainPage.transform.GetChild(2).gameObject;
        //kidObjective.SetActive(false);
        //if (kidObjective.activeInHierarchy)
        //    Debug.Log("Yasss");
        //else
        //    Debug.Log("noooo..");       
    }

    public static void checkconditions(List<InventoryItem> list)
    {
        if (!completedAllObjectives)
        {
            foreach (InventoryItem item in list)
            {
                if (item.itemData.itemName == "Snacks")
                {
                    ToDoManager.instance.CompleteItem("SnacksforRoad");
                    completedobjectives++;
                }
                else if (item.itemData.itemName == "Cigarettes" || item.itemData.itemName == "PocketKnife" || item.itemData.itemName == "BloodPawz CD")
                {
                    print("detected taht i picked up lore item, should check off to do list");
                    ToDoManager.instance.CompleteItem("SpecialBroItem");
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

    public void StartAction(string action)
    {

        switch (action)
        {
            case "kidQuest":
                if (!completedKidQuest)
                {
                    InventoryManager.instance.Add(knife);
                    journalNotif?.Invoke();
                    completedKidQuest = true;
              
                }
                break;
            case "StartkidQuest":

                Debug.Log("Giving kid quest");
                //kidObjective.SetActive(true);
                //if (kidObjective)
                //    Debug.Log("KID QUEST IS ACTIVE");
                //else
                //    Debug.Log("NOPE NOT TO BE FOUND");
                //journalNotif?.Invoke();

                break;

            case "StartbroQuest":
                journalNotif?.Invoke();

                break;


        }
    }
}
