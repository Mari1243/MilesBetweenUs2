using UnityEngine;
using UnityEngine.EventSystems;


public class emptyClickDetector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        print("click off");
        if (DragItem.currentlySelected != null)
        {
            DragItem.currentlySelected.SetSelected(false);
        }

    }
}
