using UnityEngine;
using System.Collections;
using MaskTransitions;
using System;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class IntroSceneManager : MonoBehaviour
{
    public Canvas journalcanvas;
    private bool journalActive;
    public DialogueRunner diaRun;

    public GameObject book;
    public GameObject Map;
    private RectTransform rect;
    private float wait = 3.5f;
    public GameObject TPCam;
    private bool hasJournal = false;
    public Item NOJournal;
    public static bool journalopen = false;
    public Button xButton;

    public GameObject instructionss;

    public static IntroSceneManager instance;
    public GameObject TutorialDialogueSystem;
    public GameObject DialogueSystem;

    private bool mapPlaced= false;


    void Awake()
    {
        instance = this;
        xButton.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        // Mapinteractable.showJournal += OpenJournalHint;
        // Mapinteractable.showJournal += FreezeCam;
        interactable.onMap += stuff;
        //DialogueManager.DialogOver += diaRun.Stop;
    }
    private void OnDisable()
    {
        // Mapinteractable.showJournal -= OpenJournalHint;
        // Mapinteractable.showJournal -= FreezeCam;
        interactable.onMap -= stuff;
        //DialogueManager.DialogOver -= diaRun.Stop;
        
    }


    //journal behavior
    public void stuff()
    {
        print("calling stuff");
            if(!journalopen)
            {
                TPCam.SetActive(false);
                instructionss.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                journalcanvas.enabled = true;
                DOTween.Restart("animateIn"); 
                DOTween.Play ("animateIn");
                journalopen = true;
                StartCoroutine(instructions1());
            }
            else
            {
                TPCam.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                DOTween.Restart("animateOut"); 
                DOTween.Play ("animateOut");
                journalopen = false;
                instructionss.SetActive(true);
                diaRun.Stop();
            }
    }

    private IEnumerator instructions1()
    {
        yield return new WaitForSeconds(2f);
        print("trying to start dialogue");
        DialogueManager.tutorialInstance.LoadDialog("StoreMap");
        DialogueManager.tutorialInstance.StartDialog();
        //call wait for map place
    }

    public void mapsnapped()
    {
        Map.transform.parent = book.transform;
        if (DialogueManager.DialogStart != null)
        {
            diaRun.Stop();
            print("turning dialogue off, dialogstart is now "+ DialogueManager.DialogStart);
        }
        StartCoroutine(waittt());
        //set bool that triggers wait for map place
    }

    private IEnumerator waittt()
    {
        hasJournal = true;
        print("ok doing journal stuff now");
        yield return new WaitForSeconds (1f);
        //make sure previous dialogue is finished playing)

        DialogueManager.tutorialInstance.LoadDialog("WhatisJournal");
        DialogueManager.tutorialInstance.StartDialog();
        //DialogueManager.instance.OnDialogOver();
        //set x button true
         xButton.gameObject.SetActive(true);
    }

   
    private void OpenJournalHint()
    {
        print("freezing stuff");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //StartCoroutine(hintroutine());
    }

  

    void OnTriggerEnter(Collider hit)
    {
        print("hitting "+ hit.gameObject.name);
        if (hit.name == "Player")
        {
            if (hasJournal)
            {
                NextScene();
            }
            else
            {
                print("doesnt have journal");
                
                DialogueManager.instance.TalkInteraction(NOJournal);
            
            }
        }
    }

    public bool HasItemByName(string itemName)
    {
        foreach (var kvp in InventoryManager.instance.itemDictionary)
        {
            if (kvp.Key.itemName == itemName) // assuming Item has an itemName field
            {
                return true;
            }
        }
        return false;
    }
    private void NextScene()
    {
        print("next scene");
        //play car door open noise
        //TransitionManager.Instance.PlayTransition(2f, 2f);
        SceneSwitch.Instance.SwitchScene("Car");
    }

}


