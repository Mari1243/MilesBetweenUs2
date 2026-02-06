using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] CinemachineCamera[] cameras;
    private CinemachineCamera currentCamera;

    private void OnEnable()
    {
        DialogueCommands.currentCam += changeCamera;
    }
    private void OnDisable()
    {
        DialogueCommands.currentCam -= changeCamera;
    }
  
 
    
    public void changeCamera(int camIndex)
    {
        currentCamera= cameras[camIndex];

        foreach (CinemachineCamera camera in cameras)
        {

            camera.enabled = camera == currentCamera;

        }

    }




}
