using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class CutSceneTrigger : MonoBehaviour
{
    //kid cutscene specific, can detect what tag is obj for specification
    public Item item;
    public DialogueManager dialogueManager;
    private CinemachineBasicMultiChannelPerlin camShake;
    public CinemachineCamera cam;
    public GameObject brother, newLocation;
    private BoxCollider brotherCollider;
     
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

        
        shakeCamera();
        StartDialogue();
        Collider trigger = GetComponent<Collider>();
        trigger.enabled = false;
        moveBrother();



    }

    private void Start()
    {
        if (newLocation && brother != null)
        {
            Debug.Log(gameObject.name);

        }
        else
        {
            return;
        }

        brotherCollider = brother.GetComponent<BoxCollider>();
        brotherCollider.enabled = false;
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
    
    public void moveBrother()
    {
        Vector3 pos = newLocation.transform.position;
        brother.transform.position = pos;
        brother.GetComponent<Animator>().Play("Armature_BigBro_Smoke");
        brotherCollider.enabled = true;
        
    }
   
}
