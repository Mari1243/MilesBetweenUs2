using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;
using System.Collections;
using Cursor = UnityEngine.Cursor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Mapinteractable : MonoBehaviour, IInteractable
{
    public static event HandleItem onTalk;
    public static event HandleItem onPickedUp;
    public delegate void HandleItem(Item itemData);
    public static event Action nextscene;

    public static event HandleHold onHold;
    public delegate void HandleHold(GameObject item, Item itemData);

    public static event Action showJournal;
    public static event Action onTalkCar;
    public Item item;

    public void Interact()
    {
        print("interact called, pickuprn");
        switch (gameObject.tag)
        {
            case "canPickUp":
                //in this script this is for the map
                Destroy(gameObject);
                onPickedUp?.Invoke(item); //called in InventoryManager
                showJournal?.Invoke();
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

                break;
            case "InventoryItem":
                onHold?.Invoke(gameObject,item);
                break;

            case "canSteal":

                Destroy(gameObject);
                onPickedUp?.Invoke(item); //called in InventoryManager
                break;

            case "coin":
                nextscene?.Invoke();
                break;

        }

    }
}