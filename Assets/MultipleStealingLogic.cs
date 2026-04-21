using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MultipleStealingLogic : MonoBehaviour
{
    public List<GameObject> stealables;
    public int StealCamInt = 0;

    private GameObject randomObj;
    private void OnEnable()
    {
        interactable.onMisc += misc;
        Interactor.OnStopStealing += misc;
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ChangeCamera.instance.changeCamera(StealCamInt);
        misc();
    }

    private void OnTriggerExit(Collider other)
    {
        //cleaning up the list
       Endbehavior();
    }

    private void Endbehavior()
    {
        ChangeCamera.instance.changeCamera(0);
        if (stealables.Count > 0)
        {
            foreach (GameObject obj in stealables)
            {
                if(obj != null)
                {
                obj.GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                stealables.Remove(obj);
                }
            
            }
        }
    }

    private void misc()
    {
        //choose a random stealable and highlight it
        if (stealables.Count > 0)
        {
            // Pick a random index
            int randomIndex = Random.Range(0, stealables.Count);
            
            // Get the GameObject at that index
            randomObj = stealables[randomIndex];
            randomObj.GetComponent<BoxCollider>().enabled = true;
            print("focused on one obj");
            //can we automatically trigger stealing on that object?
        }
        else
        {
            Endbehavior();
        }
    }

    private void ranout()
    {
        ChangeCamera.instance.changeCamera(0);
    }

    private void doneStealing(bool bol)
    {
        if(bol == false)
        {
            stealables.Remove(randomObj);
            misc();
            print("done stealing calling misc");
        }
    }

}
