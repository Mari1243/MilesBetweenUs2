using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class CarSceneManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();
    public GameObject tabholder;

    [Header("Cursor")]
    public Texture2D cursor;
    public Image crosshair;

    [Header("Journal")]
    public GameObject journalItem;
    public static bool journalActive;
    public GameObject DraggableItemPrefab;
    public GameObject spawningRange;
    public DragItem DG;


    [Header("Player Input")]
    public PlayerInput playerInp;

    [Header("Camera")]
    public GameObject playercam;

    [Header("Brother")]
    public Animator brother;

    [Header("Radio")]
    public List<AudioClip> radioClips = new List<AudioClip>();
    private bool isPlaying = false;


    private void OnEnable()
    {
        interactable.showJournal += JournalScene;
        interactable.onInteract += playRadio;
        ToggleJournal.hideJournal += ExitJournal;

    }
    private void OnDisable()
    {
        interactable.showJournal -= JournalScene;
        interactable.onInteract -= playRadio;
        ToggleJournal.hideJournal -= ExitJournal;
    }

    

    private void Start()
    {
        brother.Play("Armature_BigBro_Drive");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentInventory = InventoryManager.instance.inventory;
        if (currentInventory.Count == 0)
        {
            print("Empty");
        }
        else
        {
            foreach (InventoryItem items in currentInventory)
            {
                if (items.itemData.itemName == "Map") 
                {
                    Debug.Log("Found item");
                    currentInventory.Remove(items);
                }

                var journalItem = Instantiate(DraggableItemPrefab, Vector3.one, Quaternion.identity); //the connecting data

                //getting tab 1
                GameObject Tab1 = tabholder.transform.GetChild(0).gameObject;
                journalItem.transform.SetParent(Tab1.transform);
                journalItem.transform.localPosition = Vector2.zero;

                //this  assigns data
                DG.itemdata=items.itemData;
              
            }
        }

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);  
    }

    

    public void JournalScene() //SHOWS JOURNAL 
    {
        print("journal scene in carscene manager activated");
        crosshair.enabled = false;
        //Turns on the journal
        Debug.Log("Showing Journal");
        journalItem.SetActive(false);

        //changes camera angle
        playercam.GetComponent<PlayerCam>().enabled = false;
        playercam.transform.DOLocalRotate(new Vector3(11f, 20f, 1f), 1f);

        //Locks the screen and enables you to interact with the journal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        journalActive = true;

    }

    public void ExitJournal() //DISABLES JOURNAL 
    {
        crosshair.enabled = true;
        if (journalActive)
        {
            journalItem.SetActive(true);

            playercam.transform.DOLocalRotate(new Vector3(13f, 70f, 4f), 1f)
                .OnComplete(() =>
                {
                    PlayerCam camScript = playercam.GetComponent<PlayerCam>();
                    camScript.enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                });

            journalActive = false;
        }

    }

    public void playRadio()
    {
        if (!isPlaying)
        {
            int randClip = Random.Range(0, radioClips.Count);
            SoundManager.Instance.PlayAudio(radioClips[randClip]);
        }
        
    }
}
