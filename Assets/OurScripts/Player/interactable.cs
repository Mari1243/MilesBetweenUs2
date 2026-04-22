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
    public Item item;
    public delegate void HandleItem(Item itemData);
    public static event HandleItem onTalk;
    public static event HandleItem onPickedUp;

    public static event Action showJournal;
    public static event Action onInteract;
    public static event Action EmptySteal;

    public static event Action onMap;
    public static event Action onEND;
    public static event Action onMisc;

    public bool aquireable = true;

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
                //why dis,,, what dis do....
                if (SceneManager.GetActiveScene().name == "GasStation" ||SceneManager.GetActiveScene().name == "DragonLand" || SceneManager.GetActiveScene().name == "School") //CHANGE IN FUTURE TO DETECT GAME STATES SO NOT SCENE NAME DEPENDENT
                {
                    onTalk?.Invoke(item); //called in DialogueManager
                
                    break;
                }
                else if (SceneManager.GetActiveScene().name == "Car")
                {
                    GameObject SM = GameObject.Find("CarSceneManager");
                    CarSceneManager sceneManager = SM.GetComponent<CarSceneManager>();
                    onTalk?.Invoke(item); //called in DialogueManager
                    
                }

                break;

            case "journal":
                Debug.Log("showJournal being invoked from interactable");
                showJournal?.Invoke(); //called in SceneManager
                break;

   
            case "canSteal":
                if (aquireable)
                {
                    print("destroying gameobject because its can steal??");
                    Destroy(gameObject);
                    onPickedUp?.Invoke(item); //called in InventoryManager
                }
                else
                {
                    EmptySteal?.Invoke();
                }
                break;

            case "canInteract":
                onInteract?.Invoke(); //called in SceneManager
                break;

            case "Map":
                onMap?.Invoke(); //called in Introscenemanager
                Destroy(gameObject);
                break;
            case "END":
                onEND?.Invoke(); //called in schoolmanager
                break;
            case "Misc":
                onMisc?.Invoke(); //called in schoolmanager
                break;     
        }
    }
  

}