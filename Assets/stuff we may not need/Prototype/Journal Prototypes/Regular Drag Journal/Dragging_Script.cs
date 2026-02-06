using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragging_Script : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private Transform originalParent;
    private int originalSiblingIndex;

    private bool beingPlaced;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    void OnDisable()
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BookEdge")
        {
            if (beingPlaced == false)
            {
                beingPlaced = true;
                originalParent = transform.parent;
                originalSiblingIndex = transform.GetSiblingIndex();
                Canvas canvas = GetComponentInParent<Canvas>();
                transform.SetParent(canvas.transform);
                transform.SetAsFirstSibling();
            }
            else
            {
                beingPlaced = false;
                transform.SetParent(originalParent);
                transform.SetSiblingIndex(originalSiblingIndex);
            }
        }
    }
}