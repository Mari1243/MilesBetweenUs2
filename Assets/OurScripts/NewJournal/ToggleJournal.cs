using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Numerics;
using System.Runtime.InteropServices;

public class ToggleJournal : MonoBehaviour
{
    public static ToggleJournal instance;
    public static event Action OnJournalOpened;
    public static event Action OnJournalClosed;
    public bool isOpen;


    [SerializeField]public static bool journalopen = false;
    [SerializeField]public bool canOpen = true;
    private Canvas canvas;
    public UnityEngine.UI.Button xbutton;
    private UnityEngine.Vector3 oldPos;
    private GameObject journalContents;
   
    [Header("Public References")]
    private CinemachineInputAxisController playerCam;
    private ThirdPersonMovement playerMovement;

    private NewJournalSave jouralstatesystem;
     
    private void Awake()
    {
        instance = this;
        canvas = this.GetComponent<Canvas>();
        canvas.enabled = false;
        journalContents = this.transform.GetChild(0).gameObject;
        oldPos = journalContents.transform.position;
        jouralstatesystem = this.GetComponent<NewJournalSave>();


    }

    private void OnEnable()
    {
        DragItem.loreDrop += closeJournal;

        InputManager.OpenJournal += journal;

        interactable.showJournal += journal;
        
        DialogueManager.DialogStart += disableJournal; //makes it so you cant open journal while in dialogue
        DialogueManager.DialogOver += enableJournal;
        DialogueCommands.diaopenJournal += animateOpen;
        DialogueCommands.diacloseJournal += journal;


    }
    private void OnDisable()
    {
        DragItem.loreDrop -= closeJournal;

        InputManager.OpenJournal -= journal;
   
        interactable.showJournal -= journal;

        DialogueManager.DialogStart -= disableJournal;
        DialogueManager.DialogOver -= enableJournal;
        DialogueCommands.diaopenJournal -= animateOpen;
        DialogueCommands.diacloseJournal -= journal;
    }

    private void Start()
    {
    }
    public void enableJournal()
    {
        canOpen = true;
        //print("enabling journal");
    }
    //called when you talk to bro
    public void disableJournal()
    {
        canOpen = false;
        //print("disabling journal");
    }


    public void closeJournal(string node)
    {
        journal();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //print("disabling journal");
        this.GetComponent<Canvas>().enabled = false;
        
        enableJournal();
    }

    public void journal()
    {
        // Inside ToggleJournal.instance.journal (whatever that method is)
       // Debug.LogError("[ToggleJournal] journal() FIRED.");
       // if (this == null) Debug.LogError("[ToggleJournal] instance is NULL at fire time!");
       // Debug.LogError("[ToggleJournal] journal() FIRED. canOpen: " + canOpen
       //+ " journalopen: " + journalopen);

        if (this.GetComponent<Canvas>() != null)
        {
            if(!journalopen && canOpen)
            {

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;

                //Debug.LogError("[ToggleJournal] Cursor state: " + Cursor.lockState
                //+ " | visible: " + Cursor.visible);
                print("animating in");
                canvas.enabled = true;
                DOTween.Restart("animateIn"); 
                DOTween.Play("animateIn");
                journalopen = true;
                Cursor.lockState = CursorLockMode.None;
                //controls player input
                OnJournalOpened?.Invoke();

                if (SceneManager.GetActiveScene().name != "Car")
                disablePlayer();
                isOpen = true;
                Debug.LogError("[ToggleJournal] Open complete. journalopen: " + journalopen);
            }
            else
            {
                print("animating out");
                canvas.enabled = false;
                DOTween.Restart("animateOut"); 
                DOTween.Play("animateOut");
                journalopen = false;
                OnJournalClosed?.Invoke(); 

                if (SceneManager.GetActiveScene().name != "Car")
                enablePlayer();
                isOpen = false;
                Debug.LogError("[ToggleJournal] Close complete. journalopen: " + journalopen);
            }
        }
        else
        {
            print("canvas is null");
        }
    }
   
    public void animateOpen()
    {
        //this is necessary to prevent the journal from trying to open until dialogue is over
        //otherwise we keep on triggering dialogue while trying to interact with journal
        StartCoroutine(waitForDialogueEnd());
    }
    private IEnumerator waitForDialogueEnd()
    {
        yield return new WaitUntil(() => !DialogueManager.tutorialInstance.dialogStarted);
    
        journalopen = false;
        canOpen = true;

        
        journal();
    }   

    public void Open()
    {
        journalopen = false;
        canOpen = true;
        InputManager.Instance.JournalOpen = false;
        journal();
    }
 

   public void disablePlayer() //THIRD PERSON
    {
        playerCam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineInputAxisController>();
        playerMovement = GameObject.Find("Player").GetComponent<ThirdPersonMovement>();
        if (playerCam != null && playerMovement != null)
        {
            playerCam.enabled = false;
            playerMovement.enabled = false;
   
        }
    }

    public void enablePlayer()
    {
        if (SceneManager.GetActiveScene().name != "Car")
        {
            playerCam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineInputAxisController>();
            playerMovement = GameObject.Find("Player").GetComponent<ThirdPersonMovement>();
            if (playerCam != null && playerMovement != null)
            {
                playerCam.enabled = true;
                playerMovement.enabled = true;
          
            }


        }
        else
        {
            return;
        }
        
    }
}
