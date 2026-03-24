using MaskTransitions;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndCutsceneEvent1 : MonoBehaviour
{
    public GasStationManager GSManager;
    public ToggleJournal journalAccess;
    public void endCutScene()

    {
        journalAccess.canOpen = true;
        StartCoroutine(endingScene());
    }


     IEnumerator endingScene()
    {
        TransitionManager.Instance.PlayStartHalfTransition(1f, .2f);
        yield return new WaitForSeconds(1f);

        ChangeCamera.instance.changeCamera(2);

        TransitionManager.Instance.PlayEndHalfTransition(1f, .2f);

        GSManager.triggerIntroCutscene();

        
    }

    
   
}
