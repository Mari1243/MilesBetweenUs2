using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using UnityEngine.SceneManagement;
public class DialogueCommands : MonoBehaviour
{

    private DialogueRunner dialogueRunner;
    //changed to not static
    public InMemoryVariableStorage yarnVariables;

    public static UnityAction<int> currentCam; //change camera position 
    public static UnityAction<string> scenename;

    public static UnityAction diaopenJournal;
    public static UnityAction diacloseJournal;
    public static event Action<bool> EndJournalState;

    public static UnityAction<string> startAction;

    public static UnityAction ENDGame;

    [Header("Inventory")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();


    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        yarnVariables = GetComponent<InMemoryVariableStorage>();

        dialogueRunner.AddCommandHandler<int>("cameraIndex", OnCamChange);
        dialogueRunner.AddCommandHandler<string>("changeScene", OnChangeScene);
        dialogueRunner.AddCommandHandler<string>("startAction", OnStartAction);
        dialogueRunner.AddCommandHandler<bool>("openJournal", OnJournalOpen);
        dialogueRunner.AddCommandHandler<bool>("end", EndGame);
        dialogueRunner.AddCommandHandler<bool>("SetEndstate", Endstate);


    }
 
    void OnCamChange(int cam)
    {
        if (currentCam != null)
        {
            currentCam(cam);
        }
            

    }

    //added print checks
    private void EndGame(bool willend)
    {
        Debug.LogError($"[DC] EndGame called. willend={willend}. ENDGame has subscribers: {ENDGame != null}");
        if (willend)
        {
            Debug.LogError("[DC] Invoking ENDGame event now.");
            if (ENDGame == null)
            Debug.LogError("[DC] *** ENDGame IS NULL — nobody is subscribed! Scene will never load. ***");
            else
            Debug.LogError($"[DC] ENDGame invocation list count: {ENDGame.GetInvocationList().Length}");
            ENDGame?.Invoke();
        }
        else
        {
            Debug.LogError("[DC] end=false branch: resetting SchoolManager.hasPlayed");
            SchoolManager.hasPlayed = false;
        }
    }
    //added more checks
    private void Endstate(bool inEndstate)
    {
    Debug.LogError($"[DC] Endstate called. inEndstate={inEndstate}. EndJournalState has subscribers: {EndJournalState != null}");
    if (inEndstate)
    {
        if (EndJournalState == null)
            Debug.LogError("[DC] *** EndJournalState IS NULL — NewJournalSave won't receive this! ***");
        EndJournalState?.Invoke(true);
    }
    else
    {
        EndJournalState?.Invoke(false);
    }
    }


     void OnJournalOpen(bool bol)
    {
        if (bol)
        {
            print("opening journal in dialogue comamnds");
            diaopenJournal?.Invoke();
        }
        else
        {
            print("closing journal in dialogue comamnds");
            diacloseJournal?.Invoke();
        }
    }
    void OnChangeScene(string scene)
    {
        if(scenename!=null)
            scenename(scene);
    }

    void OnStartAction(string action)
    {
        if (action!=null)
            startAction(action);
            
    }

    public void checkInventory()
    //find a place to better implement
    //decided to put make this manually triggered by dialogue command
    //called in events in dialogue manager in gas station
    {
        //Debug.Log("checkInventory called from: " + System.Environment.StackTrace);
        if (yarnVariables == null)
        {
        //Debug.LogError("yarnVariables is null! DialogueCommands may not have initialized.", this);
        yarnVariables = GetComponent<InMemoryVariableStorage>(); // attempt recovery
        }
         List<InventoryItem> currentInventory = InventoryManager.instance.inventory;
         //Debug.Log("Checking inventory! Count: " + currentInventory.Count);

        if (currentInventory.Count >= 1)
        {
            foreach (InventoryItem item in currentInventory)
            {
                //Debug.Log("Inventory has this item: " + item.itemData.itemName);
                if (item.itemData.itemName == "Snacks") //name specific 
                {
                    yarnVariables.SetValue("$hasSnacks", true);
                }
                else if (item.itemData.itemName == "Lollipop") //name specific 
                {
                    yarnVariables.SetValue("$hasPostcard", true);

                }
                else if (item.itemData.itemName == "Jacket Patch") //name specific 
                {
                    print("found jacket patch");
                    yarnVariables.SetValue("$didDragonLandThing", true);

                }
                else if (item.itemData.itemName == "An ID!") //name specific 
                {
                    yarnVariables.SetValue("$hasID", true);

                }
                else if(item.itemData.itemName=="Someone's number!")
                {
                    yarnVariables.SetValue("$hasNumber", true);
                }
                 else if(item.itemData.itemName=="Dragon Plushie")
                {
                    yarnVariables.SetValue("$canLeave", true);
                }
            }
        }
        else
        {
            return;
        }

    }

 
}
