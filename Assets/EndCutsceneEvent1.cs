using UnityEngine;

public class EndCutsceneEvent1 : MonoBehaviour
{
    public GasStationManager GS;
  public void endCutScene()
    {
        ChangeCamera.instance.changeCamera(2);
        GS.triggerIntroCutscene();
    }
}
