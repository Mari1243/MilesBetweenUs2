using UnityEngine;

public class DragonLandManager : MonoBehaviour
{
    public Item startCutScene;
    public GameObject bro;
    public Animator car;
    private void Start()
    {
        bro.SetActive(false);
        car.Play("DLCar");
    }
    public void triggerIntroCutscene()
    {
        bro.SetActive(true);
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
