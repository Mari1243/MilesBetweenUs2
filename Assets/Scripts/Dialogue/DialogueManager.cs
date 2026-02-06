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

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private DialogueRunner dialogueRunner;
    public static UnityAction DialogStart, DialogOver;

    public bool dialogReady, dialogStarted;
    public Canvas dialoguePos;
    void Awake()
    {

        if (instance == null)
            instance = this;

    }

    private void OnEnable()
    {
        interactable.onTalk += TalkInteraction;
       // DialogOver += GiveItem; 
       //^^^^^^^^^^^ DialogOver event doesn't take in itemdata
    }
   

    private void OnDisable()
    {
        interactable.onTalk -= TalkInteraction;
       
        // DialogOver -= GiveItem;
        //^^^^^^^^^^^ DialogOver event doesn't take in itemdata

    }

    private void Start()
    {
        dialogueRunner = GetComponent<DialogueRunner>();

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
            dialogueRunner.Stop();

            dialogueRunner.StartDialogue(dialogueRunner.startNode);
            if (DialogStart != null)
                DialogStart();

            dialogStarted = true;
        }
    }
    public void OnDialogOver()
    {
        if (DialogStart != null)
            DialogOver();

        dialogStarted = false;

    }

    public void TalkInteraction(Item itemdata) //enables cutscene 
    {
        Debug.Log("Talking rn" + itemdata.name);
        
        dialoguePos.transform.localPosition = itemdata.diagPos;
        LoadDialog(itemdata.node);
        StartDialog();
        
    }

    
}