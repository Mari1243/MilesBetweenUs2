using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BrotherInteractable : MonoBehaviour
{

    private BrotherInteractable instance;
    public static event Action askedAbtAllLoreItems;
    public interactable interactableData;
    public Item newItem;
    public List<string> barks = new List<string>();
    private int barkCount, barkIndex;
    public Image img;
    public int minWait, maxWait;


    bool car1=false;

    private int carScene;
    //bool car1=false;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        DialogueManager.DialogOver += StartTimer;
        DialogueManager.DialogStart += CloseBubble;
        DragItem.loreDrop += loreDropping;
        SceneTrackerSingleton.onSceneName += PreviousScene;
    }
    private void OnDisable()
    {
        DialogueManager.DialogStart -= CloseBubble;
        DialogueManager.DialogOver -= StartTimer;
        SceneTrackerSingleton.onSceneName -= PreviousScene;
        DragItem.loreDrop -= loreDropping;
    }

    public void PreviousScene(string scene)
    {
        Debug.Log("THIS WAS PREVIOUS SCENE " + scene);
        ////take scene tracker singleton instance here (string) and 
        if (carScene==1)
        {
            print("last scene was gas station");
            interactableData.item = newItem;
            
        }
    }


    //listens to if dialogue is not playing and when dialogue is over and executes coroutine 

    //randomizes time between notification pop up

    //changes interactable item data node

    //pause timer

    //talk UI notification appears and hovers near brother 

    private void Start()
    {
        carScene = SceneTrackerSingleton.Instance.carnum;
        switch (carScene)
        {
            case 1:

                interactableData.item.node = "Car0Start";
                barks.Clear();
                barks.Add("Car0Bark1");
                barks.Add("Car0Bark2");
                barks.Add("Car0Bark3");
                DialogueManager.instance.TalkInteraction(interactableData.item);
                break;
            case 2:
                interactableData.item.node = "Car1Start";
                barks.Clear();
                barks.Add("Car1Bark1");
                barks.Add("Car1Bark2");
                barks.Add("Car1Bark3");
                DialogueManager.instance.TalkInteraction(interactableData.item);

                break;
            case 3:
                interactableData.item.node = "Car2Start";
                barks.Clear();
                barks.Add("Car2Bark1");
                barks.Add("Car2Bark2");
                barks.Add("Car2Bark3");
                DialogueManager.instance.TalkInteraction(interactableData.item);

                break;
        }

    }

    private void StartTimer()
    {

     
            StartCoroutine(BreakTimer());

        



    }
    private IEnumerator BreakTimer()
    {
        //problem with camera, make sure camera is in default mode before enabling this coroutine


        //also make it so he "barks" only like twice but each time between then is randomized 
        int rand = UnityEngine.Random.Range(minWait, maxWait);

        switch (carScene)
        {
            case 1:

            barkCount = barks.Count;
            //how do I space these out? Or quanitfy how many times the brother speaks to you? Also make this a public reference so you can tweak it 

                interactableData.item.node = "Car0";
                break;
            case 2:
                interactableData.item.node = "Car1";
                break;
            case 3:
                interactableData.item.node = "Car2";
                break;
        }

                barkCount = barks.Count;
            //how do I space these out? Or quanitfy how many times the brother speaks to you? Also make this a public reference so you can tweak it 

            yield return new WaitForSeconds(rand);

        if (barkCount == 0)
        {
            yield break;
        }
        else if (!DialogueManager.instance.dialogStarted )
        {
            AnimateBubble();
            barkIndex = UnityEngine.Random.Range(0, barks.Count);
            interactableData.item.node = barks[barkIndex]; //change the node in the scriptable obj 
            barks.Remove(barks[barkIndex]);


        }

    }

    public void loreDropping(string node)
    {

        print("brother interactable lore drop happened");
        //print("loredropping in broter interactable "+ newItem.name);
        if(newItem.node != null)
        {
            newItem.node = node; //change the node in the scriptable obj 
        }
        else
        {
            print("aaaa?? no node");
        }
      
        DialogueManager.instance.TalkInteraction(newItem);
        Debug.Log(newItem.diagPos);
        
    }

    public void endjournal()
    {
        DialogueManager.DialogOver += OnBrotherDialogueFinished;
    }

    private void OnBrotherDialogueFinished()
    {
        //wait for dialogue to end 
        askedAbtAllLoreItems?.Invoke();
        DialogueManager.DialogOver -= OnBrotherDialogueFinished;
    }

    private void AnimateBubble()
    {
        img.enabled = true;
        img.transform.DOScale(new Vector3(1,1,1), .5f).SetEase(Ease.OutCirc);
    }
    private void CloseBubble()
    {
        img.transform.DOScale(new Vector3(0, 0, 0), .2f).SetEase(Ease.InBounce);

    }
}
