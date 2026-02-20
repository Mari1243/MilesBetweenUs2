using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BrotherInteractable : MonoBehaviour
{
    public interactable interactableData;
    public Item newItem;
    public List<string> barks = new List<string>();
    private int barkCount, barkIndex;
    public Image img;
    public int minWait, maxWait;
    private void OnEnable()
    {
        DialogueManager.DialogOver += StartTimer;
        DialogueManager.DialogStart += CloseBubble;
        DragItem.loreDrop += loreDropping;
    }
    private void OnDisable()
    {
        DialogueManager.DialogStart -= CloseBubble;
        DialogueManager.DialogOver -= StartTimer;
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
        //problem with camera, make sure camera is in default mode before enabling this coroutine


        //also make it so he "barks" only like twice but each time between then is randomized 

        barkCount = barks.Count;
        int rand = Random.Range(minWait, maxWait); //how do I space these out? Or quanitfy how many times the brother speaks to you? Also make this a public reference so you can tweak it 
        yield return new WaitForSeconds(rand);


        if (barkCount == 0)
        {
            yield break;
        }
        else if (!DialogueManager.instance.dialogStarted )
        {
            AnimateBubble();
            barkIndex = Random.Range(0, barks.Count);
            interactableData.item.node = barks[barkIndex]; //change the node in the scriptable obj 
            barks.Remove(barks[barkIndex]);


        }
        
    }

    public void loreDropping()
    {
        newItem.node = "CDLOREDROP"; //change the node in the scriptable obj 
        DialogueManager.instance.TalkInteraction(newItem);
    }

    private void AnimateBubble()
    {
        img.enabled = true;
        img.transform.DOScale(new Vector3(1,1,1), .5f).SetEase(Ease.OutCirc);
    }
    private void CloseBubble()
    {
        img.transform.DOScale(new Vector3(0, 0, 0), .5f).SetEase(Ease.InBounce);

    }
}
