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
    
    
   
}
