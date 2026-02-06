using Unity.VisualScripting;
using UnityEngine;

//Add a check for what the tag of the item is, needs to be CollageItem to be rotated.... weird glitch resulting occasionally otherwise 
public class Rotate_Script : MonoBehaviour
{
    public static Rotate_Script Instance;
    private bool isRotating = false;
    private bool isScaling = false;
    private Vector3 lastMousePosition;
    private static GameObject selectedObject;
    public GameObject rotationSymbol;
    private GameObject temp;

    private Vector3 initialScale;

    private void OnEnable()
    {
        InputManager.onRotateChanged += OnRotateChanged;
        InputManager.onScaleChanged += OnScaleChanged;
    }

    private void OnDisable()
    {
        InputManager.onRotateChanged -= OnRotateChanged;
        InputManager.onScaleChanged -= OnScaleChanged;
    }

     public void OnScaleChanged(bool scaling)
    {
        if (scaling && selectedObject != null)
        {
            isScaling = true;
            initialScale = selectedObject.transform.localScale;
            print("Scaling started");
        }
        else
        {
            isScaling = false;
            print("Scaling stopped");
        }
    }

    public static void selectObject(GameObject obj)
    {
        if(obj.tag== "CollageItem")
        {
            selectedObject = obj;
        }
        else
        {
            print("trying to select an object you shouldnt? (naughty!)");
        }
        
    }

private void OnRotateChanged(bool rotating)
{
    if (InputManager.canRotate)
    {
        // Get sprite renderer bounds for world-space corners
        SpriteRenderer sr = selectedObject.GetComponent<SpriteRenderer>();
        Bounds b = sr.bounds;

        // Upper-right corner in world space (max x, max y)
        Vector3 upperRight = new Vector3(b.max.x, b.max.y, b.center.z);

        // Instantiate at correct world position
        temp = Instantiate(rotationSymbol, upperRight, Quaternion.identity);

        // Save original scale before parenting (important!)
        Vector3 originalScale = temp.transform.localScale;

        // Parent it but KEEP WORLD POSITION
        temp.transform.SetParent(selectedObject.transform, true);

        // Restore original scale to avoid shrinking
        temp.transform.localScale = originalScale;

        // Make sure render order is above target sprite
        temp.GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;

        isRotating = true;
    }
    else
    {
        print("DONE ROTATING");
        isRotating = false;
        KilltheKids();
    }
}

    private void KilltheKids()
    {
        foreach(Transform child in selectedObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Then in Update or wherever you need it:
    private void Update()
    {
        if (InputManager.canRotate || isScaling)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            
            // Calculate mouse delta once for both operations
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            
            // Handle rotation if active
            if (InputManager.canRotate && isRotating)
            {
                HandleRotationMovement(mouseDelta);
            }
            
            // Handle scaling if active
            if (isScaling)
            {
                HandleScaleMovement(mouseDelta);
            }
            
            // Update last position once at the end
            lastMousePosition = currentMousePosition;
        }
    }
private void HandleRotationMovement(Vector3 mouseDelta)
    {
        //print("handling rotation movement");
        if (selectedObject != null && isRotating)
        {
            if (mouseDelta != Vector3.zero)
            {
                float rotationSpeed = 20.0f;
                float rotationAmount = -mouseDelta.x * rotationSpeed * Time.deltaTime;
                selectedObject.transform.Rotate(0, 0, rotationAmount);
            }
        }
    }
    
    private void HandleScaleMovement(Vector3 mouseDelta)
    {
        if (selectedObject != null && InputManager.canScale)
        {
            if (mouseDelta != Vector3.zero)
            {
                // Use vertical mouse movement for scaling
                float scaleSpeed = 20f;
                float scaleChange = mouseDelta.y * scaleSpeed * Time.deltaTime;
                
                // Get current scale and apply change
                Vector3 newScale = selectedObject.transform.localScale + Vector3.one * scaleChange;
                
                // Optional: Clamp the scale to prevent it from going negative or too large
                float minScale = 45f;
                float maxScale = 180f;
                newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
                newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
                //this way z can never really be scaled up
                newScale.z = Mathf.Clamp(newScale.z, minScale, 5.5f);
                
                selectedObject.transform.localScale = newScale;
                
                print($"Scaling: {newScale}");
            }
        }
    }
}
