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

public class ToggleJournal : MonoBehaviour
{
    public static event Action OnJournalOpened;
    public static event Action OnJournalClosed;


    [SerializeField]public static bool journalopen = false;
    [SerializeField]public bool canOpen = false;
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


    }
    private void OnDisable()
    {
        DragItem.loreDrop -= closeJournal;

        InputManager.OpenJournal -= journal;
   
        interactable.showJournal -= journal;

        DialogueManager.DialogStart -= disableJournal;
        DialogueManager.DialogOver -= enableJournal;
    }

    private void Start()
    {
        canOpen = false;
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
        //unfreeze input
        //set mouse inactive
        journalopen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //print("disabling journal");
        this.GetComponent<Canvas>().enabled = false;
        
        enableJournal();
    }

    public void journal()
    {
       if(this.GetComponent<Canvas>() != null)
        {
            if(!journalopen && canOpen)
    {
        canvas.enabled = true;
        DOTween.Restart("animateIn"); 
        DOTween.Play("animateIn");
        journalopen = true;
        Cursor.lockState = CursorLockMode.None;
        OnJournalOpened?.Invoke(); // add this

        if (SceneManager.GetActiveScene().name != "Car")
        disablePlayer();
    }
    else
    {
        canvas.enabled = false;
        DOTween.Restart("animateOut"); 
        DOTween.Play("animateOut");
        journalopen = false;
        OnJournalClosed?.Invoke(); // add this

    if (SceneManager.GetActiveScene().name != "Car")
        enablePlayer();
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
    
        jouralstatesystem.SetState(States.Car);
        xbutton.gameObject.SetActive(false);
        journalopen = false;
        canOpen = true;
        journal();
    }   

    public void Open()
    {
        journalopen = false;
        canOpen = true;
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
