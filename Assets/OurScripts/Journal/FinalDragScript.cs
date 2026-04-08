using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FinalDragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //should be assigned on save called when drag ends
    public Sprite sprite;
    public Vector3 endlocation;
    public Vector3 endrotation;
    public string parentname;
    public String endname;

    private SpriteRenderer DropShadow;

    private bool itemsaved;

    public LayerMask dragPlaneLayer = -1; // Layer for invisible drag plane
    public float snapDistance = 0.5f; // Distance threshold for snapping

    [Header("Surface Constraints")]
    public bool constrainToSurface = false;
    public Vector2 surfaceBounds = new Vector2(5f, 5f); // Local bounds on the surface

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Transform parentSurface;
    private Plane dragPlane;
    private Vector3 originalLocalPosition;
    private bool isSelected;
    private bool isSaved;

    private Vector3 ogSize;
    private Vector3 selectedSize;

    void Awake()
    {
        // Save item info
        
        var spriterenderer = GetComponent<SpriteRenderer>();
        //sprite = spriterenderer.sprite;f
        endlocation = gameObject.transform.localPosition;
        endrotation = gameObject.transform.localEulerAngles;
        
        endname = gameObject.name.ToString();

        // Check if an item with this name already exists in the list (for current session)
        FinalDragScript existingItem = JournalSave.items.Find(item => item != null && item.endname == this.endname && item != this);
    
        if (existingItem != null)
        {
        // Remove the old reference and replace with this one (updated position)
        JournalSave.items.Remove(existingItem);
        //Debug.Log("Replaced existing item in save list: " + this.endname);
        }
    
        // Add this item to the list
        JournalSave.items.Add(this);
        //Debug.Log("Added item to save list: " + this.endname);
    }

    void Start()
    {
        validateSetup();
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();

        parentSurface = transform.parent;
        originalLocalPosition = transform.localPosition;

        // Create a plane aligned with the parent's surface (assuming Z-forward is the surface normal)
        UpdateDragPlane();

        parentname = gameObject.transform.parent.name.ToString();

        DropShadow = this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Sprite newspr = this.GetComponent<SpriteRenderer>().sprite;
        DropShadow.sprite = newspr; // Directly set the sprite property
        print(this.GetComponent<SpriteRenderer>().sprite.name);
        print(DropShadow.sprite.name);
        DropShadow.enabled = false;

    }

    private void validateSetup()
    {
        // Check for collider (required for EventSystem to detect clicks/drags)
        Collider col2D = GetComponent<Collider>();
        if (col2D == null)
        {
            //Debug.LogWarning($"Adding BoxCollider2D to {gameObject.name} for drag detection");
            BoxCollider2D boxCol = gameObject.AddComponent<BoxCollider2D>();
            boxCol.isTrigger = true; // Make it a trigger so it doesn't interfere with physics
        }

        // Ensure there's a Physics2DRaycaster on the camera
        if (mainCamera != null)
        {
            Physics2DRaycaster raycaster = mainCamera.GetComponent<Physics2DRaycaster>();
            if (raycaster == null)
            {
                //Debug.LogWarning("Adding Physics2DRaycaster to main camera for 2D drag detection");
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            }
        }
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("No EventSystem found in scene!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //print("selected an image called " + gameObject.name);
        // Toggle selection state
        SetSelected(!isSelected);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();


        // Deselect other stickers when this one is selected
        if (selected)
        {
            FinalDragScript[] allStickers = FindObjectsOfType<FinalDragScript>();
            foreach (FinalDragScript sticker in allStickers)
            {
                if (sticker != this)
                {
                    sticker.SetSelected(false);
                }
                else
                {
                    Rotate_Script.selectObject(gameObject);
                }
            }
        }
    }

    void UpdateVisualState()
    {
        gameObject.transform.localScale = isSelected ? selectedSize : ogSize;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // print("beginning drag");
        StartDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            DragSprite();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        Rotate_Script.selectObject(gameObject);
        isDragging = true;
        UpdateDragPlane();
        DropShadow.enabled = true;
        print(DropShadow.sprite);
        // Calculate offset between mouse position and sprite position in world space
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;

        // Optional: Scale up slightly during drag for visual feedback
        transform.localScale *= 1.1f;
    }

    void DragSprite()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetWorldPos = mouseWorldPos + dragOffset;

        // Convert world position to parent's local space
        Vector3 targetLocalPos = parentSurface.InverseTransformPoint(targetWorldPos);

        // Constrain to surface (keep Z at 0 in local space, assuming surface is XY plane)
        targetLocalPos.z = originalLocalPosition.z;

        // Apply surface bounds constraints
        if (constrainToSurface)
        {
            //will keep it from going off the edges the book
            //targetLocalPos.x = Mathf.Clamp(targetLocalPos.x, -surfaceBounds.x / 2, surfaceBounds.x / 2);
            //targetLocalPos.y = Mathf.Clamp(targetLocalPos.y, -surfaceBounds.y / 2, surfaceBounds.y / 2);
        }

        // Apply the constrained local position
        transform.localPosition = targetLocalPos;
        ///added for experimentation
        SnapToGrid();
    }

    void EndDrag()
    {
        isDragging = false;

        // Snap to grid or specific positions if needed
        SnapToGrid();
        DropShadow.enabled = false;
        // Reset scale
        transform.localScale /= 1.1f;
    }

    Vector3 GetMouseWorldPosition()
    {
        // Cast a ray from camera through mouse position onto the drag plane
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        // Fallback: use screen to world conversion at current depth
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    void UpdateDragPlane()
    {
        if (parentSurface == null) return;

        // Create a plane that's aligned with the parent's surface
        // Assuming the parent's forward vector is the surface normal
        Vector3 surfacePoint = parentSurface.position;
        Vector3 surfaceNormal = parentSurface.forward;

        dragPlane = new Plane(surfaceNormal, surfacePoint);
    }

    void SnapToGrid()
    {
        if (snapDistance <= 0) return;

        Vector3 localPos = transform.localPosition;

        // Snap to grid in local space
        localPos.x = Mathf.Round(localPos.x / snapDistance) * snapDistance;
        localPos.y = Mathf.Round(localPos.y / snapDistance) * snapDistance;

        transform.localPosition = localPos;
        save();
    }

    public void save()
    {
        // Saving sprite
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        sprite = spriteRenderer.sprite;
        
        // Saving location (FIXED: was saving x for z coordinate)
        endlocation = new Vector3(
            gameObject.transform.position.x, 
            gameObject.transform.position.y, 
            gameObject.transform.position.z
        );
        
        // Saving rotation (FIXED: was saving x for z coordinate)
        endrotation = new Vector3(
            gameObject.transform.eulerAngles.x, 
            gameObject.transform.eulerAngles.y, 
            gameObject.transform.eulerAngles.z
        );
        
        // Saving parent
        parentname = gameObject.transform.parent.name.ToString();
        
        // Setting name
        endname = gameObject.name.ToString();
    }
}