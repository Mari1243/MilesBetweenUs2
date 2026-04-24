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
    
    //for this levels fetchquest
    [SerializeField]private bool completedGardenerQuest = false;
    public Item GardenerReward;
    public Item FratItem;
    public Item SchoolPamphlet;

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
        DialogueCommands.startAction += StartAction;
        DialogueCommands.ENDGame += ENDINGTHEGAME;

    }
    void OnDisable()
    {
        interactable.onEND -= triggerEND;
        InventoryManager.OnInventoryChange -= checkconditions;
        DialogueCommands.startAction -= StartAction;
        DialogueCommands.ENDGame -= ENDINGTHEGAME;
    }

    private void giftItem(Item itemData)
    {
        InventoryManager.instance.Add(itemData);
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



        }
    }

    private void ENDINGTHEGAME()
    {

        ToggleJournal.instance.journal();
        SceneManager.LoadScene("EndDemoScene");
    }

    private IEnumerator endroutine()
    {
        ToggleJournal.instance.journal();
        yield return new WaitForSeconds (1f);
    }
}
