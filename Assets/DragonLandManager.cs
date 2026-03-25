using UnityEngine;

public class DragonLandManager : MonoBehaviour
{
    public Item startCutScene;
    public Item MouseReward;
    public GameObject bro;
    public Animator car;
    private bool completedMouseQuest = false;


    private void OnEnable()
    {
        DialogueCommands.startAction += StartAction;

    }

    private void OnDisable()
    {
        DialogueCommands.startAction -= StartAction;

    }
    private void Start()
    {
        bro.SetActive(false);
        car.Play("DLCar");
    }
    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        Debug.Log("Playing cutscene");
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StartAction(string action)
    {

        switch (action)
        {
            case "mouseQuest":
                if (!completedMouseQuest)
                {
                    InventoryManager.instance.Add(MouseReward);
                    completedMouseQuest = true;
                }
                break;
        


        }
    }
}
