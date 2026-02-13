using UnityEngine;
using UnityEngine.EventSystems;


public class emptyClickDetector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        print("click off");
        DragItem.currentlySelected.SetSelected(false);


    }
}
