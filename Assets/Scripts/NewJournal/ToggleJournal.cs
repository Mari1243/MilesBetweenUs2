using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToggleJournal : MonoBehaviour
{
    public Image crosshair;
    public static event Action hideJournal;
    private bool journalopen = false;
    private Canvas canvas;

    private void Start()
    {

        canvas = this.GetComponent<Canvas>();
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        // interactable.showJournal += JournalScene;
        DragItem.loreDrop += closeJournal;
        InputManager.OpenJournal += inventory;
        //ToggleJournal.hideJournal += inventory;
        interactable.showJournal += inventory;
    }
    private void OnDisable()
    {
        // interactable.showJournal -= JournalScene;
        DragItem.loreDrop -= closeJournal;
        InputManager.OpenJournal -= inventory;
        //ToggleJournal.hideJournal -= inventory;
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

   

}
