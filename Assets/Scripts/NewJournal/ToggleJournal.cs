using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;

public class ToggleJournal : MonoBehaviour
{
    public Image crosshair;
    public static event Action hideJournal;
    private void Start()
    {
        this.GetComponent<Canvas>().enabled = false;
    }

    private void OnEnable()
    {
        interactable.showJournal += JournalScene;
        DragItem.loreDrop += closeJournal;
    }
    private void OnDisable()
    {
        interactable.showJournal -= JournalScene;
        DragItem.loreDrop -= closeJournal;
    }

    public void closeJournal()
    {
        //unfreeze input
        //set mouse inactive
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        print("disabling journal");
        this.GetComponent<Canvas>().enabled = false;
        hideJournal?.Invoke();
    }

    public void JournalScene() //SHOWS JOURNAL 
    {
        print("showing journal");
        this.GetComponent<Canvas>().enabled = true;
    }

}
