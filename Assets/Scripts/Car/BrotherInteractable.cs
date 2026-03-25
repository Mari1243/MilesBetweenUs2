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
    public Item DLPrompt;
    public Item EndPrompt;
    public List<string> barks = new List<string>();
    private int barkCount, barkIndex;
    public Image img;
    public int minWait, maxWait;
    bool car1,car2=false;
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
        if (scene == "GasStation")
        {
            print("last scene was gas station");
            interactableData.item = DLPrompt;
            car1 = true;
        }
        else if(scene == "DragonLand")
        {
            interactableData.item = EndPrompt;
            Debug.Log("Last scene was dragonland");
            car1 = false;
            car2 = true;
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
        if (car1)
            interactableData.item.node = "Car1";
        else if(car2)
            interactableData.item.node = "Car2";

        else
            interactableData.item.node = "Car0";


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

    public void loreDropping(string node)
    {
        DLPrompt.node = node; //change the node in the scriptable obj 
        DialogueManager.instance.TalkInteraction(DLPrompt);
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
