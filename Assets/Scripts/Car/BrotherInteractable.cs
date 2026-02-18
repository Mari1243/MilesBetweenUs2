using NUnit.Framework;
using UnityEngine;

public class BrotherInteractable : MonoBehaviour
{
    public interactable interactableData;
    public Item newItem;
   
    private void Start()
    {


        if (SceneTrackerSingleton.Instance.PreviousSceneName == "GasStation")
        {
            interactableData.item = newItem;
        }
    }
    
    



}
