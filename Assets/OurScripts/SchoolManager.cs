using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class SchoolManager : MonoBehaviour
{
    private WaitForSeconds wait = new WaitForSeconds(1f);
    public GameObject physicalJournal;
    private bool hasPlayed=false;
    public DialogueRunner diaRun;

    private void Start()
    {
        physicalJournal.SetActive(false);
    }

    void OnEnable()
    {
        interactable.onEND += triggerEND;
        DialogueCommands.diaopenJournal+= openjournal;
    }
    void OnDisable()
    {
        interactable.onEND += triggerEND;
    }

    private void triggerEND()
    {
        //only do once
        if (!hasPlayed)
        {
            hasPlayed = true;
            print("game over yayyyy");
            StartCoroutine(endanimation());
        }
    } 

    private IEnumerator endanimation()
    {
        yield return wait;
        //call a cutscene cam3 here
        //trigger dialogue with bro
        DialogueManager.tutorialInstance.LoadDialog("EndDialogue1");
        DialogueManager.tutorialInstance.StartDialog();
        //show journal object
        physicalJournal.SetActive(true);
       
        // if (DialogueManager.DialogStart != null)
        // {
        //     diaRun.Stop();
        //     print("turning dialogue off, dialogstart is now "+ DialogueManager.DialogStart);
        // }
    }

    private void openjournal()
    {
        StartCoroutine(slowjournalopen());

    } 

    private IEnumerator slowjournalopen()
    {
        yield return new WaitForSeconds (2.5f);
                print("OPEENING JOURNALLLL");
         //make clickable to open and edit
        ToggleJournal journalToggle = Object.FindAnyObjectByType<ToggleJournal>();
        
        journalToggle.animateOpen();
        print("toggle is "+ journalToggle.gameObject);
        //open journal in car state (without x button)
    }
}
