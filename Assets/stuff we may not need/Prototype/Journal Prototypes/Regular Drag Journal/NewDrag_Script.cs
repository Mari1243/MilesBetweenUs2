using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewDrag_Script : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    //making public variables/settings
    [Header("Grid Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private bool snapToGrid = true;
    [SerializeField] private bool showGridGizmos = true;
   
    [Header("Selection")]
    [SerializeField] private GameObject selectionIndicator;
    //[SerializeField] private Color selectedColor = Color.white;
    //[SerializeField] private Color normalColor = Color.grey;
   
    private Vector3 dragOffset;
    private Camera mainCamera;
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    private Vector3 ogSize;
    private Vector3 selectedSize;
   
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
           
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure we have required components
        ValidateSetup();
        UpdateVisualState();
    }
    
    void ValidateSetup()
    {
        //getting the original size & selected size
        ogSize = gameObject.transform.localScale;
        selectedSize = new Vector3(ogSize.x *2f, ogSize.y *2, ogSize.z);

        // Check for collider (required for EventSystem to detect clicks/drags)
        Collider2D col2D = GetComponent<Collider2D>();
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
                Debug.LogWarning("Adding Physics2DRaycaster to main camera for 2D drag detection");
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            }
        }
        
        // Check for EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("No EventSystem found in scene!");
        }
    }
    //only called if there isnt a selection indicator yet!
  
   
    public void OnPointerClick(PointerEventData eventData)
    {
        print("old script pointer click works");
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
            NewDrag_Script[] allStickers = FindObjectsOfType<NewDrag_Script>();
            foreach (NewDrag_Script sticker in allStickers)
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
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isSelected);
        }
    }
   
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mainCamera == null) 
        {
            Debug.LogError("No main camera found!");
            return;
        }
        
        // Convert screen position to world position
        Vector3 screenPos = eventData.position;
        screenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
       
        dragOffset = transform.position - worldPos;
        originalPosition = transform.position;
       
        // Select this sticker when starting to drag
        SetSelected(true);

    }
   
    public void OnDrag(PointerEventData eventData)
    {
        if (mainCamera == null) return;
        
        // Convert screen position to world position
        Vector3 screenPos = eventData.position;
        screenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
       
        Vector3 targetPosition = worldPos + dragOffset;
        targetPosition.z = transform.position.z; // Maintain Z position
       
        if (snapToGrid)
        {
            targetPosition = SnapToGrid(targetPosition);
        }
       
        transform.position = targetPosition;
        
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        
        // Final snap to grid if enabled
        if (snapToGrid)
        {
            transform.position = SnapToGrid(transform.position);
        }
    }
   
    Vector3 SnapToGrid(Vector3 position)
    {
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
       
        return new Vector3(snappedX, snappedY, position.z);
    }
   
    // Method to toggle grid snapping on/off
    public void ToggleGridSnap()
    {
        snapToGrid = !snapToGrid;
    }
   
    // Method to set grid size dynamically
    public void SetGridSize(float newGridSize)
    {
        gridSize = Mathf.Max(0.1f, newGridSize);
    }

    // Visual feedback for grid in Scene view
    
    void OnDrawGizmos()
    {
        if (!showGridGizmos) return;
       
        Gizmos.color = UnityEngine.Color.gray * 0.5f;
       
        // Draw grid around this object's position
        Vector3 pos = transform.position;
        int gridRange = 10;
       
        // Draw vertical lines
        for (int x = -gridRange; x <= gridRange; x++)
        {
            float xPos = Mathf.Round(pos.x / gridSize) * gridSize + (x * gridSize);
            Vector3 start = new Vector3(xPos, pos.y - gridRange * gridSize, pos.z);
            Vector3 end = new Vector3(xPos, pos.y + gridRange * gridSize, pos.z);
            Gizmos.DrawLine(start, end);
        }
       
        // Draw horizontal lines
        for (int y = -gridRange; y <= gridRange; y++)
        {
            float yPos = Mathf.Round(pos.y / gridSize) * gridSize + (y * gridSize);
            Vector3 start = new Vector3(pos.x - gridRange * gridSize, yPos, pos.z);
            Vector3 end = new Vector3(pos.x + gridRange * gridSize, yPos, pos.z);
            Gizmos.DrawLine(start, end);
        }
    }
   
    // Properties for external access
    public bool IsSelected => isSelected;
    public float GridSize => gridSize;
    //public bool SnapToGrid => snapToGrid;
}

// Optional: Manager class to handle global grid settings
[System.Serializable]
public class GridSettings
{
    public float globalGridSize = 1f;
    public bool globalSnapEnabled = true;
    public KeyCode toggleSnapKey = KeyCode.G;
}

public class StickerGridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;

    void Update()
    {
        // Toggle grid snapping globally with a key
        if (Input.GetKeyDown(gridSettings.toggleSnapKey))
        {
            gridSettings.globalSnapEnabled = !gridSettings.globalSnapEnabled;

            NewDrag_Script[] allStickers = FindObjectsOfType<NewDrag_Script>();
            foreach (NewDrag_Script sticker in allStickers)
            {
                // Update each sticker's snap setting
                if (gridSettings.globalSnapEnabled)
                    sticker.SetGridSize(gridSettings.globalGridSize);
            }

            Debug.Log($"Grid Snap: {(gridSettings.globalSnapEnabled ? "ON" : "OFF")}");
        }
    }

    public void SetGlobalGridSize(float size)
    {
        gridSettings.globalGridSize = size;

        NewDrag_Script[] allStickers = FindObjectsOfType<NewDrag_Script>();
        foreach (NewDrag_Script sticker in allStickers)
        {
            sticker.SetGridSize(size);
        }
    }

}