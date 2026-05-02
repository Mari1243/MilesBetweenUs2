using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using MaskTransitions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;


public class SchoolManager : MonoBehaviour
{
    [Header("End")]
    private WaitForSeconds wait = new WaitForSeconds(1f);
    public GameObject physicalJournal, FX, Player;
    public static bool hasPlayed=false;
    public DialogueRunner diaRun;
    [SerializeField] GameObject endInteractable;
    

    [Header("Objectives")]
    [SerializeField]private bool completedAllObjectives;
    public int allobjectives = 1;
    private int completedobjectives = 0;

    [Header("Intro")]
    public Item startCutScene;
    public GameObject bro;
    public Animator car;
    public Transform startPos, endPos;

    [Header("Rewards")]
    [SerializeField]private bool completedGardenerQuest = false;
    public Item GardenerReward;
    public Item FratItem;
    public Item SchoolPamphlet;
    

    private void Start()
    {
        bro.transform.position = startPos.position;
        physicalJournal.SetActive(false);
         bro.SetActive(false);
        car.Play("DLCar");
        
    }


    //added checks
    void OnEnable()
    {
        interactable.onEND += triggerEND;
        InventoryManager.OnInventoryChange += checkconditions;
        DialogueCommands.startAction += StartAction;
        DialogueCommands.ENDGame += ENDINGTHEGAME;
        Debug.LogError("SchoolManager Subscribed to ENDGame.");
        DialogueCommands.EndJournalState += ENDJOURNAL;

    }
    void OnDisable()
    {
        interactable.onEND -= triggerEND;
        InventoryManager.OnInventoryChange -= checkconditions;
        DialogueCommands.startAction -= StartAction;
        DialogueCommands.ENDGame -= ENDINGTHEGAME;
        DialogueCommands.EndJournalState -= ENDJOURNAL;

        Debug.LogError("schoolmanager *** UNSUBSCRIBED from ENDGame — if this fires before end, scene load is gone! ***");
    }


    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //added checks
   private void triggerEND()
{
    Debug.LogError($"[SM] triggerEND called. hasPlayed={hasPlayed}");
    if (!hasPlayed)
    {
        hasPlayed = true;
        
        // Disable the END interactable so it can never fire again this session
        if (endInteractable != null)
        {
            endInteractable.SetActive(false);
            Debug.LogError("[SM] END interactable disabled.");
        }
        else
        {
            Debug.LogError("[SM] *** endInteractable reference is null — assign it in inspector! ***");
        }
        
        StartCoroutine(endanimation());
    }
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
                if (item.itemData.itemName == "Someone's number!")
                {
                    if (ToDoManager.instance == null) { Debug.LogError("ToDoManager instance is null!"); return; }
                    ToDoManager.instance.CompleteItem("GardenerQuest");
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

   //added checks
   private IEnumerator endanimation()
    {
        Debug.LogError("[SM] endanimation: activating physicalJournal.");
        physicalJournal.SetActive(true);
        yield return wait;
        Debug.LogError("[SM] endanimation: wait complete. Loading EndDialogue1.");
        Debug.LogError($"[SM] DialogueManager.tutorialInstance null? {DialogueManager.tutorialInstance == null}");
        DialogueManager.tutorialInstance.LoadDialog("EndDialogue1");
        DialogueManager.tutorialInstance.StartDialog();
        Debug.LogError("[SM] endanimation: StartDialog called.");
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
    //also any time something changes with the to do list the check journal/todolist popup should be called

    public void StartAction(string action)
    {
        switch (action)
        {
            case "gardenerQuest":
                if (!completedGardenerQuest)
                {
                    InventoryManager.instance.Add(GardenerReward);
                    ToDoManager.instance.CompleteItem("GardenerQuest");
                    completedGardenerQuest = true;
                }
           
                break;
            case "StartgardenerQuest":
                ToDoManager.instance.spawnnewToDoTask("GardenerQuest", "Get a girls number");

                break;
            case "Frat":
                InventoryManager.instance.Add(FratItem);
                ToDoManager.instance.CompleteItem("CheckoutFratBros");
                break;
            case "Pamphlet":
                InventoryManager.instance.Add(SchoolPamphlet);
                ToDoManager.instance.CompleteItem("FlyerforMax");
                break;
            case "moveBro":
                bro.transform.position = endPos.position;
                bro.transform.rotation = endPos.rotation;
                bro.GetComponent<Animator>().Play("Armature_BigBro_SIT");
                Debug.Log("switching sides");
                
                break;


        }
    }
    //added checks
    private void ENDINGTHEGAME()
    {
        StartCoroutine(triggerendscene());
    }   
    private IEnumerator triggerendscene()
    {
        TransitionManager.Instance.PlayEndHalfTransition(1f, .2f);

        yield return new WaitForSeconds(2f); // let DOTween finish
        Debug.LogError("[SM] Loading EndCutscene now.");

        SceneManager.LoadScene("CutsceneEND");
    }
    private void ENDJOURNAL(bool end)
    {
        physicalJournal.SetActive(false);
        FX.SetActive(false);
        Player.SetActive(false);
    }
}
