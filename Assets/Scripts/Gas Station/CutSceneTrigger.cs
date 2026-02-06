using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class CutSceneTrigger : MonoBehaviour
{

    public Item item;
    public DialogueManager dialogueManager;
    private CinemachineBasicMultiChannelPerlin camShake;
    public CinemachineCamera cam;
    private void OnEnable()
    {
        DialogueManager.DialogOver += unShakeCamera;
    }
    private void OnDisable()
    {
        DialogueManager.DialogOver -= unShakeCamera;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (gameObject.tag == "Border")
        {
            StartDialogue();
        }
 
        else
        {
            shakeCamera();
            StartDialogue();
            Collider trigger = GetComponent<Collider>();
            trigger.enabled = false;
        }

            
    }

    private void Start()
    {
        camShake = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

    }
    public void shakeCamera() //ThirdPerson Scene Specific
    {

        if (camShake)
        {

            camShake.enabled = true;
            camShake.FrequencyGain = 10;
        }
        else
        {
            return;
        }

    }
    public void unShakeCamera() //ThirdPerson Scene Specific
    {

        if (camShake)
        {

            camShake.enabled = false;
            camShake.FrequencyGain = 0;
        }
        else
        {
            return;
        }
      
        
    }

    public void StartDialogue()
    {
        dialogueManager.TalkInteraction(item);
    }
   
    public void broDialogue()
    {
        GameObject camera = GameObject.Find("CameraHolder");
        GameObject playercam = camera.transform.GetChild(0).gameObject;

        playercam.transform.DOLocalRotate(new Vector3(0f, 8.5f, 0f), 1f);

        playercam.transform.DOLocalMoveZ(1f, 1f);
        playercam.GetComponent<PlayerCam>().enabled = false;
        StartDialogue();
    }
    public void loadNextScene()
    {
        SceneManager.LoadScene("StateMachineWork");
    }
   
}
