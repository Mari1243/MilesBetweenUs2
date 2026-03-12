using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
public class DialogueCommands : MonoBehaviour
{

    private DialogueRunner dialogueRunner;
    public static InMemoryVariableStorage yarnVariables;

    public static UnityAction<int> currentCam; //change camera position 
    public static UnityAction<string> scenename;

    public static UnityAction<string> startAction;
    [Header("Inventory")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();


    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        yarnVariables = GetComponent<InMemoryVariableStorage>();

        dialogueRunner.AddCommandHandler<int>("cameraIndex", OnCamChange);
        dialogueRunner.AddCommandHandler<string>("changeScene", OnChangeScene);
        dialogueRunner.AddCommandHandler<string>("startAction", OnStartAction);
        

        
    }

   
    void OnCamChange(int cam)
    {
        if (currentCam != null)
            currentCam(cam);

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
    //called in events in dialogue manager in gas station
    {
        currentInventory = InventoryManager.instance.inventory;

        Debug.Log("Checking inventory!");
        if (currentInventory.Count >= 1)
        {
            foreach (InventoryItem item in currentInventory)
            {
                Debug.Log("Inventory has this item: " + item.itemData.itemName);
                if (item.itemData.itemName == "Snacks") //name specific 
                {
                    Debug.Log("Found!");
                    yarnVariables.SetValue("$hasSnacks", true);
                }
                else if (item.itemData.itemName == "Lollipop") //name specific 
                {
                    yarnVariables.SetValue("$hasPostcard", true);

                }
            }
        }
        else
        {
            return;
        }

    }

 
}
