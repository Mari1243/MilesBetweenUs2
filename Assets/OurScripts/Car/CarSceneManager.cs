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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class CarSceneManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();
    //public GameObject tabs;

    [Header("Cursor")]
    public Texture2D cursor;
    public Image crosshair;

    [Header("Journal")]
    public GameObject journalItem;
    public static bool journalActive;
    //public GameObject DraggableItemPrefab;
    //public DragItem DG;


    [Header("Player Input")]
    public PlayerInput playerInp;

    [Header("Camera")]
    public GameObject playercam;

    [Header("Brother")]
    public Animator brother;
    public Animator lilBro;

    [Header("Radio")]
    public List<AudioClip> radioClips = new List<AudioClip>();
    private bool isPlaying = false;

    [Header("SceneChanges")]
    private int carScene;
    public GameObject treeSpawners;

    private void OnEnable()
    {
        interactable.showJournal += JournalScene;
        interactable.onInteract += playRadio;
        ToggleJournal.OnJournalClosed += ExitJournal;
        DialogueManager.DialogOver += ExitJournal;
        SceneTrackerSingleton.carOver -= ClearInventory;



    }
    private void OnDisable()
    {
        interactable.showJournal -= JournalScene;
        interactable.onInteract -= playRadio;
        ToggleJournal.OnJournalClosed -= ExitJournal;
        DialogueManager.DialogOver -= ExitJournal;
        SceneTrackerSingleton.carOver += ClearInventory;

    }



    private void Start()
    {

        //Animations play
        brother.Play("Armature_BigBro_Drive");
        lilBro.Play("Armature|LilGuy_CarRide");

        //Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
 

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

        carScene = SceneTrackerSingleton.Instance.carnum;
        environmentChange();

    }



    public void JournalScene() //SHOWS JOURNAL 
    {
        print("journal scene in carscene manager activated (changes camera stuff)");
        crosshair.enabled = false;
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

    public void environmentChange() //if we wanna change the exterior how it looks 
    {
        switch (carScene)
        {
            case 1: //pre gas station
                break;
            case 2: //pre dragon land
                Debug.Log("Treeeeeees gone");
                treeSpawners.SetActive(false);
                break;
            case 3: //pre college
                break;
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

    public void ClearInventory()
    {
        currentInventory.Clear();
        InventoryManager.instance.inventory.Clear();
    }
}
