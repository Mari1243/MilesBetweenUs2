using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] CinemachineCamera[] cameras;
    public CinemachineCamera currentCamera;
    public static ChangeCamera instance;
    public bool hasCutscene;

    private void OnEnable()
    {
        DialogueCommands.currentCam += changeCamera;
    }
    private void OnDisable()
    {
        DialogueCommands.currentCam -= changeCamera;
    }

    private void Start()
    {
        currentCamera = GetComponent<CinemachineBrain>().GetComponent<CinemachineCamera>();
        if (hasCutscene)
        {
            currentCamera= cameras[0];
            currentCamera = cameras[cameras.Length - 1];
            print(currentCamera.name);

            foreach (CinemachineCamera camera in cameras)
            {

            camera.enabled = camera == currentCamera;

            }

        }
    }

    void Awake()
    {

        if (instance == null)
            instance = this;

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
