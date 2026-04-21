using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public static DialogueManager tutorialInstance;
    [SerializeField] private bool isTutorialSystem = false;
    private DialogueRunner dialogueRunner;
    public static UnityAction DialogStart, DialogOver;

    public bool dialogReady, dialogStarted;
    public Canvas dialoguePos;
    void Awake()
    {
       if (isTutorialSystem)
        {
            tutorialInstance = this;
            //print(tutorialInstance.name);
        }
        else
        instance = this;
        dialogueRunner=GetComponent<DialogueRunner>();
    }

    private void OnEnable()
    {
        if (!isTutorialSystem)
        {
            interactable.onTalk += TalkInteraction;
        }
       
       // DialogOver += GiveItem; 
       //^^^^^^^^^^^ DialogOver event doesn't take in itemdata
    }
   

    private void OnDisable()
    {
         if (!isTutorialSystem)
        {
            interactable.onTalk -= TalkInteraction;
        }
       
        // DialogOver -= GiveItem;
        //^^^^^^^^^^^ DialogOver event doesn't take in itemdata

    }


    public void LoadDialog(string node)
    {
        dialogueRunner.startNode = node;
        dialogReady = true;
    }

    public void StartDialog()
    {
        if (dialogReady && !dialogStarted)
        {
            //print("dialogue is ready and dialogue isnt started, lets start");
            dialogueRunner.Stop();
            //print(dialogueRunner.name);
            dialogueRunner.StartDialogue(dialogueRunner.startNode);
            if (DialogStart != null)
                DialogStart();

            dialogStarted = true;
        }
        else
        {
            //print("somethig is wrong, " + dialogReady+ dialogStarted);
        }
    }
    public void OnDialogOver()
    {
        //print("calling dialogue over");
        //print("the journal state (ToggleJournal.journalopen) is now "+ ToggleJournal.journalopen);
        if (DialogStart != null)
            DialogOver();

        dialogStarted = false;

    }

    public void TalkInteraction(Item itemdata) //enables cutscene 
    {
        //this is what is being used for hte journal
        //Debug.Log("Talking rn" + itemdata.name);
        //print("position is " + itemdata.diagPos);
        //print("gonna play node "+ itemdata.node);
        
        dialoguePos.transform.localPosition = itemdata.diagPos;
        LoadDialog(itemdata.node);
        StartDialog();
        
    }

    public void StopDialogue()
    {
        dialogueRunner.Stop();
      
    }
}