using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class Tabbutton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    public TabGroup tabGroup;
    public UnityEngine.UI.Image background;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background = this.GetComponent<UnityEngine.UI.Image>();
        tabGroup.Subscribe(this);
    }
    public void select()
    {
        onTabSelected.Invoke();
    }
    public void deselect()
    {
        onTabDeselected.Invoke();
    }
}
