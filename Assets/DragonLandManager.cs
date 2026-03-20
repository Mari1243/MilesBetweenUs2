using UnityEngine;

public class DragonLandManager : MonoBehaviour
{
    public Item startCutScene;

    public void triggerIntroCutscene()
    {
        DialogueManager.instance.TalkInteraction(startCutScene);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
