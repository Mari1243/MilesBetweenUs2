using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;
using System.Collections;
using Cursor = UnityEngine.Cursor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


/*
 * This script handles all events basically and detects what interactable item's tag is and the corresponding event will invoke in that script
 */
public class interactable : MonoBehaviour, IInteractable
{


    public static event HandleItem onTalk;
    public static event HandleItem onPickedUp;
    public delegate void HandleItem(Item itemData);
    public static event HandleItem coinPickedUp;

    public static event HandleHold onHold;
    public delegate void HandleHold(GameObject item, Item itemData);

    public static event Action showJournal;
    public static event Action onTalkCar;
    public Item item;


    public void Interact()
    {

        switch (gameObject.tag)
        {
            case "canPickUp":

                Destroy(gameObject);
                onPickedUp?.Invoke(item); //called in InventoryManager
                break;

            case "canTalk":

                //first finds if there's an outline and will disable it 
                if (gameObject.transform.GetComponent<Outline>())
                {
                    gameObject.transform.GetComponent<Outline>().enabled = false;
                }

                if (SceneManager.GetActiveScene().name == "GasStation" || SceneManager.GetActiveScene().name == "GS_CursorV0"|| SceneManager.GetActiveScene().name == "GS_CursorV1") //CHANGE IN FUTURE TO DETECT GAME STATES SO NOT SCENE NAME DEPENDENT
                {
                    onTalk?.Invoke(item); //called in DialogueManager
                
                    break;
                }
                else if (SceneManager.GetActiveScene().name == "Car")
                {
                    GameObject SM = GameObject.Find("CarSceneManager");
                    CarSceneManager sceneManager = SM.GetComponent<CarSceneManager>();
                    //if (sceneManager.isHoldingItem)
                    //{
                    //    onTalkCar?.Invoke(); //called in CarSceneManager
                    //}
                    //else
                    //{
                        onTalk?.Invoke(item); //called in DialogueManager
                    
                }

                break;

            case "journal":
                Debug.Log("showingjournal");
                showJournal?.Invoke(); //called in SceneManager
                break;

            case "InventoryItem":
                onHold?.Invoke(gameObject,item);
                break;

            case "canSteal":

                Destroy(gameObject);
                onPickedUp?.Invoke(item); //called in InventoryManager
                break;

            case "coin":
                Debug.Log("coin picked up!");
                Destroy(gameObject);
                coinPickedUp?.Invoke(item);
                break;

        }
    }
  

}