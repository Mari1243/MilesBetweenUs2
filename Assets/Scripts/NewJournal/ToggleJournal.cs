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
    private bool journalopen = false;
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

        InputManager.OpenJournal += inventory;
        InputManager.OpenJournal += disablePlayer;
        hideJournal += enablePlayer;
        interactable.showJournal += inventory;
        
    }
    private void OnDisable()
    {
        DragItem.loreDrop -= closeJournal;

        InputManager.OpenJournal -= inventory;
        InputManager.OpenJournal -= disablePlayer;
        hideJournal -= enablePlayer;
        interactable.showJournal -= inventory;
        
    }

    public void closeJournal(string node)
    {
        //unfreeze input
        //set mouse inactive
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        print("disabling journal");
        this.GetComponent<Canvas>().enabled = false;
        hideJournal?.Invoke();
    }

    public void inventory()
    {
        print("calling animations and journal stuff from UI manager");
        print("asdffff");
       if(this.GetComponent<Canvas>() != null)
        {
            if(!journalopen)
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
