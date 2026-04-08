using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public float distanceFromTop = 50f; // Pixels above the top edge

    private Canvas canvas;
    private RectTransform canvasRect;
    private RectTransform parentRect;
    private RectTransform myRect;
    
    // Rotation state
    private float initialAngle;
    private Quaternion initialRotation;

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
        print("clicked on rotator");
    }

    void PositionHandle()
    {
        if (parentRect == null || myRect == null) return;
        
        // Get the top center of the parent in world space
        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);
        
        // Corners are: 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right
        Vector3 topCenter = (corners[1] + corners[2]) / 2f;
        
        // Get the up direction of the rotated parent
        Vector3 upDirection = (corners[1] - corners[0]).normalized;
        
        // Position handle above the top edge
        Vector3 handleWorldPos = topCenter + upDirection * distanceFromTop;
        
        // Use transform.position instead of myRect.position
        transform.position = handleWorldPos;
    }   

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;
        
        // Store initial rotation
        initialRotation = parentRect.rotation;
        
        // Calculate initial angle from parent center to mouse
        initialAngle = GetAngleToMouse(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;

        // Calculate current angle
        float currentAngle = GetAngleToMouse(eventData.position);
        
        // Calculate delta and apply rotation
        float deltaAngle = currentAngle - initialAngle;
        parentRect.rotation = initialRotation * Quaternion.Euler(0, 0, deltaAngle);

        // Update handle position as parent rotates
        PositionHandle();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Drag ended, nothing special needed
    }

    float GetAngleToMouse(Vector2 screenPoint)
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

        // Calculate angle
        Vector2 direction = localPoint - parentLocalPoint;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}