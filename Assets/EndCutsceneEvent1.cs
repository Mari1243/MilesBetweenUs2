using MaskTransitions;
using System.Collections;
using UnityEngine;

public class EndCutsceneEvent1 : MonoBehaviour
{
    public GasStationManager GS;
  public void endCutScene()
    {
        StartCoroutine(endingScene());
    }


     IEnumerator endingScene()
    {
        TransitionManager.Instance.PlayStartHalfTransition(.2f, .2f);
        yield return new WaitForSeconds(1f);

        ChangeCamera.instance.changeCamera(2);

        TransitionManager.Instance.PlayEndHalfTransition(.2f, .2f);

        GS.triggerIntroCutscene();
        
    }
}
