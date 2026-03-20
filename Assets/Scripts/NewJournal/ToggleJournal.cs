using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class ToggleJournal : MonoBehaviour
{
    public static event Action hideJournal;
    [SerializeField]private bool journalopen = false;
    [SerializeField] private bool canOpen = false;
    private Canvas canvas;
   
    [Header("Public References")]
    public CinemachineInputAxisController playerCam;
    public ThirdPersonMovement playerMovement;
     
    private void Start()
    {

        canvas = this.GetComponent<Canvas>();
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        DragItem.loreDrop += closeJournal;

        InputManager.OpenJournal += journal;
        InputManager.OpenJournal += disablePlayer;

        interactable.showJournal += journal;
        
        DialogueManager.DialogStart += disableJournal; //makes it so you cant open journal while in dialogue
        DialogueManager.DialogOver += enableJournal;
    }
    private void OnDisable()
    {
        DragItem.loreDrop -= closeJournal;

        InputManager.OpenJournal -= journal;
        InputManager.OpenJournal -= disablePlayer;
   
        interactable.showJournal -= journal;

        DialogueManager.DialogStart -= disableJournal;
        DialogueManager.DialogOver -= enableJournal;
    }

    public void enableJournal()
    {
        canOpen = true;
    }
    public void disableJournal()
    {
        canOpen = false;
    }


    public void closeJournal(string node)
    {
        //unfreeze input
        //set mouse inactive
        journalopen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        print("disabling journal");
        this.GetComponent<Canvas>().enabled = false;
        
        enablePlayer();
        enableJournal();
    }

    public void journal()
    {
        print("calling animations and journal stuff from UI manager");
        print("asdffff");
       if(this.GetComponent<Canvas>() != null)
        {
            if(!journalopen&&canOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canvas.enabled = true;
                DOTween.Restart("animateIn"); 
                DOTween.Play ("animateIn");
                journalopen = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                DOTween.Restart("animateOut"); 
                DOTween.Play ("animateOut");
                journalopen = false;
                hideJournal?.Invoke();
                
            }
        }
    }
   

    public void onAnimCompleted()
    {
        if( journalopen == true)
        {
            
        }
        else
        {
            canvas.enabled = false;
            Time.timeScale = 1;
        }
    }

   public void disablePlayer()
    {
        playerCam.enabled = false;
        playerMovement.enabled = false;
    }

    public void enablePlayer()
    {
        if (SceneManager.GetActiveScene().name != "Car")
        {

            playerCam.enabled = true;
            playerMovement.enabled = true;
        }
        else
        {
            return;
        }
        
    }
}
