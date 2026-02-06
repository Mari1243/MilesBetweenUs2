using UnityEngine;

public class SetCamera : MonoBehaviour
{

    public int CamIndex;
    public int ExitCamIndex;
    public ChangeCamera changeCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            changeCamera.changeCamera(CamIndex);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")){
            changeCamera.changeCamera(ExitCamIndex);

        }
        
    }
}
