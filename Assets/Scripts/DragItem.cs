using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine.Events;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Drag Settings")]
    [SerializeField] private float dragThreshold = 5f; // Pixels before it counts as drag
    
    private bool isSelected;
    private bool isDragging;
    private Vector3 dragStartPos;
    private Vector3 dragOffset;
    
    private Shadow dropShadow;
    private Canvas canvas;
    private GameObject rotationIcon;
    private GameObject scaleIcon;
    private Camera mainCamera;

    private bool lorePlaced=false;
    public static UnityAction<string> loreDrop;
    public string itemNode;


    // Static reference to currently selected item
    public static DragItem currentlySelected;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
        
        ValidateSetup();

        canvas = GetComponentInParent<Canvas>();
        rotationIcon = transform.GetChild(0).gameObject;
        scaleIcon = transform.GetChild(1).gameObject;
        rotationIcon.SetActive(false);
        scaleIcon.SetActive(false);

        dropShadow = GetComponent<Shadow>();
        if (dropShadow != null)
            dropShadow.enabled = false;
    }
    // OnPointerDown - just record the starting position
    public void OnPointerDown(PointerEventData eventData)
    {
        print("pointer down");
        dragStartPos = Input.mousePosition;
    }

    public void DeselectCurrent()
    {
        //the line that will call this is gonna be:
        
        SetSelected(false);
    }

    // OnPointerClick - only fires for actual clicks (not drags)
    public void OnPointerClick(PointerEventData eventData)
    {
        print("im clicking");
        // This event only fires if the pointer hasn't moved beyond threshold
        if (!isDragging)
        {
            Debug.Log($"Click detected on {name}");
            SetSelected(true);
        }
        if (lorePlaced && this.gameObject.tag == "LoreItem")
        {
            loreDrop(itemNode);
        }
        
    }

    private void ValidateSetup()
    {
        // For UI elements, we need GraphicRaycaster on Canvas, not Physics2DRaycaster
        if (canvas != null)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogWarning($"Adding GraphicRaycaster to Canvas for UI raycasting");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        
        // Ensure the Image/Graphic has raycast target enabled
        Graphic graphic = GetComponent<Graphic>();
        if (graphic != null && !graphic.raycastTarget)
        {
            Debug.LogWarning($"Enabling raycastTarget on {name}");
            graphic.raycastTarget = true;

        }
        Image img = GetComponent<Image>();
        if(img != null)
        {
            img.maskable = true;
        }
        
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("No EventSystem found in scene!");
        }
    }



public void SetSelected(bool selected)
    {
        if (selected)
        {
            // Deselect previous selection
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.SetSelected(false);
            }
            
            // Set this as current selection
            currentlySelected = this;
            print("currently selecting " + this.name);
            isSelected = true;
            
            rotationIcon.SetActive(true);
            scaleIcon.SetActive(true);
            if (dropShadow != null)
                dropShadow.enabled = true;
                
            transform.SetAsLastSibling();
            Debug.Log($"Selected: {name}");
        }
        else
        {
            isSelected = false;
            rotationIcon.SetActive(false);
            scaleIcon.SetActive(false);
            if (dropShadow != null)
                dropShadow.enabled = false;
                
            if (currentlySelected == this)
            {
                currentlySelected = null;
            }
            
            Debug.Log($"Deselected: {name}");
        }
    }
     public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if we've moved beyond threshold
        float distance = Vector3.Distance(Input.mousePosition, dragStartPos);
        
        if (distance > dragThreshold)
        {
            //currentlySelected.GetComponent<Image>().maskable = false;
            transform.SetParent(canvas.transform);

            isDragging = true;
            dragOffset = transform.position - Input.mousePosition;
            
            // Auto-select when starting to drag
            if (!isSelected)
            {
                SetSelected(true);
            }
            
            if (dropShadow != null)
                dropShadow.enabled = true;
                
            Debug.Log($"Drag started on {name}");
        }
    }


    void StartDrag()
    {
        isDragging = true;
        // Calculate offset in screen space for UI elements
        dragOffset = (Vector2)transform.position - (Vector2)Input.mousePosition;
    }

public void OnDrag(PointerEventData eventData)
{
    if (isDragging)
    {
    Vector3 targetPosition = Input.mousePosition + (Vector3)dragOffset;
    transform.position = targetPosition;
    }
}

public void OnEndDrag(PointerEventData eventData)
{
    if (isDragging)
    {
        EndDrag();
    }
}

void EndDrag()
{
    isDragging = false;
    dropShadow.enabled = false;
    DetectImageBelow();
}

    public void DetectImageBelow()
    {
        print("detecting image");
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        bool foundCollageItem = false;
        
        foreach (RaycastResult result in results)
        {
            //print("layered on a thing");
            if (result.gameObject != gameObject && result.gameObject.CompareTag("Page")) 
            {
                print("parenting bc its tagged "+ result.gameObject.tag);
                // This is the image underneath
                transform.SetParent(result.gameObject.transform);
                foundCollageItem = true;

                checkLoreItem(this.gameObject); //Detects if lore item is on the page, if so, call function 
                
                break;
            }
            else
            {
                print("whats underneath is " + result.gameObject.name);
            }
        }
        
        // Only call notinjournal if we didn't find any CollageItem
        if (!foundCollageItem)
        {
            notinjournal();
        }
    }

    public void checkLoreItem(GameObject item)
    {
        lorePlaced = true;
        if (item.gameObject.tag == "LoreItem")
        {
            if (item.gameObject.GetComponent<OutlineUI>() != null)
            {
                
                item.gameObject.GetComponent<OutlineUI>().effectColor = Color.yellow;



            }
            else
            {
                item.gameObject.AddComponent<OutlineUI>();
                item.gameObject.GetComponent<OutlineUI>().effectColor = Color.yellow;
            }
        }
    }


    private void notinjournal()
    {
        print("calling not in journal, deparenting");
        transform.parent = canvas.transform;
    }
}


