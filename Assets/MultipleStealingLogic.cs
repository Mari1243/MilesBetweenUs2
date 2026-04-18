using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MultipleStealingLogic : MonoBehaviour
{
    public List<GameObject> stealables;
    public int StealCamInt;

    private GameObject randomObj;
    private void OnEnable()
    {
        interactable.onMisc += misc;
        StealingManager.OnStealingActionChanged+=doneStealing;
        Interactor.OnHoldCanceled += ranout;
    }
    private void OnDisable()
    {
        interactable.onMisc -= misc;
        StealingManager.OnStealingActionChanged-=doneStealing;
        Interactor.OnHoldCanceled -= ranout;
    }

    private void Start()
    {
        foreach (GameObject obj in stealables)
        {
            obj.GetComponent<BoxCollider>().enabled = false;
            //this sets their default end cam that it switches to when ur done stealing to this cam
            obj.GetComponent<StealableItemBehavior>().defaultCameraInt = StealCamInt;
        }
    }

    private void misc()
    {
        

        //change cam freeze movement
        ChangeCamera.instance.changeCamera(StealCamInt);
        //choose a random stealable and highlight it
        if (stealables.Count > 0)
        {
            // Pick a random index
            int randomIndex = Random.Range(0, stealables.Count);
            
            // Get the GameObject at that index
            randomObj = stealables[randomIndex];
            randomObj.GetComponent<BoxCollider>().enabled = true;
            print("focused on one obj");
        }
        else
        {
            //end;
            
        }
    }

    private void ranout()
    {
        //leave
    }

    private void doneStealing(bool bol)
    {
        if(bol == false)
        {
            misc();
        }
    }

}
