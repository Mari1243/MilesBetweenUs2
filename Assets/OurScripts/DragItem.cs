using DG.Tweening;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Drag Settings")]
    [SerializeField] private float dragThreshold = 5f; // Pixels before it counts as drag
    
    private bool isSelected;
    private bool isDragging;
    private Vector3 dragStartPos;
    private Vector3 dragOffset;

    public float desiredWidth = 1000;
    public float iconsize = 2500;
    
    private Shadow dropShadow;
    private Canvas canvas;
    private GameObject rotationIcon;
    private GameObject scaleIcon;

    private bool lorePlaced=false;
    private bool loreDone = false;
    public static UnityAction<string> loreDrop;
    public Item itemdata;
    public Image loreIcon;
    // Static reference to currently selected item
    public static DragItem currentlySelected;

    public Material loreMat;
    private Image sprRen;

    private void Start()
    {
        if(loreIcon != null)
        {
            loreIcon.enabled = false;
        }

        canvas = GetComponentInParent<Canvas>();
        ValidateSetup();

       
        rotationIcon = transform.GetChild(0).gameObject;
        scaleIcon = transform.GetChild(1).gameObject;
        rotationIcon.SetActive(false);
        scaleIcon.SetActive(false);

        dropShadow = GetComponent<Shadow>();
        if (dropShadow != null)
            dropShadow.enabled = false;
        this.GetComponent<Image>().sprite = itemdata.img;
        fixSizing();
    }
    // OnPointerDown - just record the starting position
    public void OnPointerDown(PointerEventData eventData)
    {
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
        // This event only fires if the pointer hasn't moved beyond threshold
        if (!isDragging)
        {
            Debug.Log($"Click detected on {name}");
            SetSelected(true);
        }
        if (lorePlaced)
        {
            print("triggering lore drop");
            //when place
            loreDrop(itemdata.node);
            lorePlaced = false;
            loreDone = true;
            //GetComponent<OutlineUI>().enabled= false;
            loreIcon.enabled = false;

            GetComponent<Image>().DOKill();
            GetComponent<Image>().DOColor(Color.white, 0f);
            //turn the material back

            
        }
        
    }

    private void fixSizing()
    {
        //scaling is required so that the proportions of the image stay the same
        if (itemdata.name != "Map")
        {
            float aspectRatio = itemdata.img.rect.height / itemdata.img.rect.width;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(desiredWidth, desiredWidth * aspectRatio);

            // *** ADD THIS: Normalize the scale after sizing ***
            this.transform.localScale = Vector3.one;
            //then you have to resize the rotation icon which got scaled with the parent
            var rotationIcon = this.transform.GetChild(0);
            rotationIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconsize, iconsize);
            var scaleIcon = this.transform.GetChild(1);
            scaleIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconsize, iconsize);
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
        
        if (Object.FindFirstObjectByType<EventSystem>() == null)
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

            sprRen = gameObject.GetComponent<Image>();
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
            if (result.gameObject != gameObject && result.gameObject.CompareTag("Page") || result.gameObject.CompareTag("TabHolder")) 
            {
                print("parenting bc its tagged "+ result.gameObject.tag);
                // This is the image underneath
                transform.SetParent(result.gameObject.transform);
                foundCollageItem = true;

                if (result.gameObject.CompareTag("Page"))
                {

                    Debug.LogError(this.name +"is the object with sprite "+ this.itemdata.img.name);
                    checkLoreItem(this.gameObject, this.itemdata.img); //Detects if lore item is on the page, if so, call function 

                }
                else
                {
                    break;
                }
            }
            else if(result.gameObject.name == "snapper")
            {
                print("whats underneath is " + result.gameObject.name);
                this.transform.position = result.gameObject.transform.position;
                IntroSceneManager.instance.mapsnapped();
                this.enabled = false;
            }
        }
        
        // Only call notinjournal if we didn't find any CollageItem
        if (!foundCollageItem)
        {
            notinjournal();
        }
    }

   public void checkLoreItem(GameObject item, Sprite newspr)
{
    if (itemdata.loreItem && !loreDone)
    {
        lorePlaced = true;
        sprRen = item.GetComponent<Image>();

        Material instanceMat = Instantiate(loreMat);
        
        Texture2D tex = newspr.texture;
        Rect rect = newspr.textureRect;
        
        Vector2 scale = new Vector2(rect.width / tex.width, rect.height / tex.height);
        Vector2 offset = new Vector2(rect.x / tex.width, rect.y / tex.height);

        // Debug everything
        Debug.LogError("Sprite name: " + newspr.name);
        Debug.LogError("Texture name: " + tex.name);
        Debug.LogError("Texture size: " + tex.width + "x" + tex.height);
        Debug.LogError("Rect: " + rect);
        Debug.LogError("Scale: " + scale);
        Debug.LogError("Offset: " + offset);
        Debug.LogError("Has Tiling property: " + instanceMat.HasProperty("Tiling"));
        Debug.LogError("Has Offset property: " + instanceMat.HasProperty("Offset"));
        Debug.LogError("Has MainText property: " + instanceMat.HasProperty("MainText"));

        instanceMat.SetTexture("MainText", tex);
        instanceMat.SetVector("Tiling", scale);
        instanceMat.SetVector("Offset", offset);

        // Verify after setting
        Debug.LogError("Tiling after set: " + instanceMat.GetVector("Tiling"));
        Debug.LogError("Offset after set: " + instanceMat.GetVector("Offset"));
        Debug.LogError("Texture after set: " + instanceMat.GetTexture("MainText"));

        sprRen.material = instanceMat;

        // Verify material on the image
        Debug.LogError("sprRen material: " + sprRen.material.name);
        Debug.LogError("sprRen material Tiling: " + sprRen.material.GetVector("Tiling"));
        Debug.LogError("sprRen material texture: " + sprRen.material.GetTexture("MainText"));

        sprRen.DOColor(Color.yellow, .7f).SetLoops(-1, LoopType.Yoyo);
        loreIcon.enabled = true;
        loreIcon.GetComponent<Image>().DOFade(1, .7f).SetLoops(-1, LoopType.Yoyo);
    }
}



    private void notinjournal()
    {
       
        if (gameObject.tag != "Map")
        {
            print("calling not in journal, deparenting");
            transform.parent = canvas.transform;
        }
        else
        {
            print("gameobject is map, parent should be fine");
            
        }

    }


}


