using UnityEngine;
using UnityEngine.EventSystems;
public class JournalSnappingScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Grid Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private Color gizmoColor = Color.white;
    
    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    
    private Transform parentTransform;
    private Vector3 initialLocalPosition;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector2 lastMouseDelta;

    public float sensitivity = 0.001f;
    
    private void Start()
    {
        parentTransform = transform.parent;
        initialLocalPosition = transform.localPosition;
        mainCamera = Camera.main ?? FindObjectOfType<Camera>();
        
        if (mainCamera == null)
        {
            Debug.LogError("No camera found! GridSnappingDraggable needs a camera to convert screen to world positions.");
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        isDragging = true;
        lastMouseDelta = Vector2.zero;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !isDraggable || mainCamera == null) return;
        
        // Get the mouse movement delta in screen space
        Vector2 mouseDelta = eventData.delta;
        
        if (parentTransform != null)
        {
            // Simple approach: scale the mouse movement by a factor based on camera distance
            float scaleFactor = Vector3.Distance(mainCamera.transform.position, transform.position) * sensitivity;
            
            // Create movement in world space (right and up directions)
            Vector3 worldDelta = mainCamera.transform.right * mouseDelta.x * scaleFactor +
                               mainCamera.transform.up * mouseDelta.y * scaleFactor;
            
            // Convert world delta to parent's local space
            Vector3 localDelta = parentTransform.InverseTransformDirection(worldDelta);
            
            // Apply movement in local space
            Vector3 newLocalPos = transform.localPosition + localDelta;
            
            // Keep Z unchanged
            newLocalPos.z = transform.localPosition.z;
            
            // Snap to grid in local space
            Vector3 snappedLocalPos = SnapToGrid(newLocalPos);
            
            // Apply the snapped local position
            transform.localPosition = snappedLocalPos;
        }
        else
        {
            // No parent - work in world space
            float scaleFactor = Vector3.Distance(mainCamera.transform.position, transform.position) * 0.001f;
            
            Vector3 worldDelta = mainCamera.transform.right * mouseDelta.x * scaleFactor +
                               mainCamera.transform.up * mouseDelta.y * scaleFactor;
            
            Vector3 newWorldPos = transform.position + worldDelta;
            Vector3 snappedWorldPos = SnapToGrid(newWorldPos);
            transform.position = snappedWorldPos;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // Final snap to ensure precision
        if (parentTransform != null)
        {
            Vector3 currentLocalPos = transform.localPosition;
            Vector3 finalSnappedPos = SnapToGrid(currentLocalPos);
            transform.localPosition = finalSnappedPos;
        }
        else
        {
            Vector3 currentWorldPos = transform.position;
            Vector3 finalSnappedPos = SnapToGrid(currentWorldPos);
            transform.position = finalSnappedPos;
        }
    }
    
    private Vector3 SnapToGrid(Vector3 position)
    {
        // Snap each component to the nearest grid point
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
        
        return new Vector3(snappedX, snappedY, position.z);
    }
}
