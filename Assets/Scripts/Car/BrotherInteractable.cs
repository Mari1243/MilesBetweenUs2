using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrotherInteractable : MonoBehaviour
{
    public interactable interactableData;
    public Item newItem;
    public List<string> barks = new List<string>();
    public int barkCount;
    private void OnEnable()
    {
        DialogueManager.DialogOver += StartTimer;
    }


    private void Start()
    {

        interactableData.item.node = "Car0";

        if (SceneTrackerSingleton.Instance.PreviousSceneName == "GasStation")
        {
            interactableData.item = newItem;
        }
    }

    
    //listens to if dialogue is not playing and when dialogue is over and executes coroutine 
    
    //randomizes time between notification pop up

    //changes interactable item data node

    //pause timer

    //talk UI notification appears and hovers near brother 



    private void StartTimer()
    {
        StartCoroutine(BreakTimer());
    }
    private IEnumerator BreakTimer()
    {
        int rand=Random.Range(10, 25); //how do I space these out? Or quanitfy how many times the brother speaks to you?
        yield return new WaitForSeconds(rand); 
            
        int randBark = Random.Range(0,barks.Count);
        interactableData.item.node=barks[randBark]; //change the node in the scriptable obj 
        //yoooo lowkey how am i getting the "barks"
           
    }



}
