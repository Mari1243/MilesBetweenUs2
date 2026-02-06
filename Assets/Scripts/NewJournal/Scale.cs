using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public float distanceFromCorner = 50f; // Pixels from bottom-right corner
    
    [Header("Scale Limits")]
    public float minScale = 0.5f;
    public float maxScale = 3f;
    
    private Canvas canvas;
    private RectTransform canvasRect;
    private RectTransform parentRect;
    private RectTransform myRect;
    
    // Scale state
    private float initialDistance;
    private Vector3 initialScale;
    
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
        
        PositionHandle();
    }
    
    void OnEnable()
    {
        if (myRect != null) // In case OnEnable fires before Start
        {
            PositionHandle();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        print("clicked on scaler");
    }
    
    void PositionHandle()
    {
        if (parentRect == null || myRect == null) return;
        
        // Get the corners of the parent in world space
        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);
        
        // Corners are: 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right
        Vector3 bottomRight = corners[3];
        
        // Get the right and down directions of the rotated parent
        Vector3 rightDirection = (corners[3] - corners[0]).normalized;
        Vector3 downDirection = (corners[0] - corners[1]).normalized;
        
        // Position handle outside the bottom-right corner
        Vector3 handleWorldPos = bottomRight + (rightDirection + downDirection) * (distanceFromCorner / 1.414f);
        
        transform.position = handleWorldPos;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;
        
        // Store initial scale
        initialScale = parentRect.localScale;
        
        // Calculate initial distance from parent center to mouse
        initialDistance = GetDistanceToMouse(eventData.position);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;
        
        // Calculate current distance
        float currentDistance = GetDistanceToMouse(eventData.position);
        
        // Calculate scale factor (avoid division by zero)
        if (initialDistance > 0.01f)
        {
            float scaleFactor = currentDistance / initialDistance;
            
            // Apply scale with limits
            Vector3 newScale = initialScale * scaleFactor;
            
            // Clamp the scale uniformly
            float scaleAmount = newScale.x / initialScale.x;
            scaleAmount = Mathf.Clamp(scaleAmount, minScale / initialScale.x, maxScale / initialScale.x);
            
            parentRect.localScale = initialScale * scaleAmount;
        }
        
        // Update handle position as parent scales
        PositionHandle();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Drag ended, nothing special needed
    }
    
    float GetDistanceToMouse(Vector2 screenPoint)
    {
        // Convert mouse position to canvas local point
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );
        
        // Convert parent position to canvas local point
        Vector2 parentLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                parentRect.position
            ),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out parentLocalPoint
        );
        print("scaling");
        // Calculate distance
        return Vector2.Distance(localPoint, parentLocalPoint);
    }
}