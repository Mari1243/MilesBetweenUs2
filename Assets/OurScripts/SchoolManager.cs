using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using MaskTransitions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SchoolManager : MonoBehaviour
{
    private WaitForSeconds wait = new WaitForSeconds(1f);
    public GameObject physicalJournal;
    public static bool hasPlayed=false;
    public DialogueRunner diaRun;

    //for todo logic
    [SerializeField]private bool completedAllObjectives;
    public int allobjectives = 1;
    private int completedobjectives = 0;

    private ToggleJournal journalToggle;


    //for intro
    public Item startCutScene;
    public GameObject bro;
    public Animator car;


    private void Start()
    {
        physicalJournal.SetActive(false);
         bro.SetActive(false);
        car.Play("DLCar");
        DialogueManager.DialogOver += startpatrol;
        
    }

    private void startpatrol()
    {
        Patrol.instance.StartPatrol();
        DialogueManager.DialogOver -= startpatrol;
    }

    void OnEnable()
    {
        interactable.onEND += triggerEND;
        InventoryManager.OnInventoryChange += checkconditions;
        BrotherInteractable.askedAbtAllLoreItems += triggerFinishedJournal;
    }
    void OnDisable()
    {
        interactable.onEND -= triggerEND;
        InventoryManager.OnInventoryChange -= checkconditions;
        BrotherInteractable.askedAbtAllLoreItems -= triggerFinishedJournal;
    }

    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void triggerEND()
    {
        if (!hasPlayed)
        {
            hasPlayed = true;
            //print("game over yayyyy");
            StartCoroutine(endanimation());
            //finding and assigning journal
            journalToggle = Object.FindAnyObjectByType<ToggleJournal>();
        }
    } 

    private void triggerFinishedJournal()
    {
        print("triggering FINISHED JOURNAL");
        //this triggeres when youve asked aout all the lore items
        DialogueManager.tutorialInstance.LoadDialog("ShowBrotherPrompt");
        DialogueManager.tutorialInstance.StartDialog();
    }

    public void checkconditions(List<InventoryItem> list)
    {
        if (!completedAllObjectives)
        {
            foreach (InventoryItem item in list)
            {
                //specific quest
                if (item.itemData.itemName == "School Pamphlet")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                   ToDoManager.instance.CompleteItem("FlyerforMax");
                    completedobjectives++;
                }
                if (item.itemData.itemName == "Fraternity Flyer")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                   ToDoManager.instance.CompleteItem("CheckoutFratBros");
                    completedobjectives++;
                }
                if (completedobjectives >= allobjectives)
                {
                    completedAllObjectives = true;
                    print("completed all level objectives yay");
                }
            }
        }
    }

    private IEnumerator endanimation()
    {
        physicalJournal.SetActive(true);
        yield return wait;

        //this presents the option to open the journal
        DialogueManager.tutorialInstance.LoadDialog("EndDialogue1");
        DialogueManager.tutorialInstance.StartDialog();
        hasPlayed = true;
        
        //try doing this with dialogue instead
        //journalToggle.animateOpen();

        //open journal in car state (without x button)
        //physicalJournal.SetActive(false);
    }

   
    public void endCutScene()

    {
        StartCoroutine(endingScene());
    }


     IEnumerator endingScene()
    {
        TransitionManager.Instance.PlayStartHalfTransition(1f, .2f);
        yield return new WaitForSeconds(1f);

        ChangeCamera.instance.changeCamera(2);

        TransitionManager.Instance.PlayEndHalfTransition(1f, .2f);

        this.triggerIntroCutscene();

        
    }


    /*fetch quests:
    fetch quest: gardener asking for number + gets you his business card
    brother lore item is business card that you get from that
    Frat flyer brother lore item
    school pamphlet
    */
}
