using UnityEngine;

public class TempENDBRO : MonoBehaviour
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
