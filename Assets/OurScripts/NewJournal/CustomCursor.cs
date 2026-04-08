using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Hide the default cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse position
        Vector2 movePos;

        // Handle different canvas render modes
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            movePos = Input.mousePosition;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out movePos);
        }

        // Update cursor position
        rectTransform.position = movePos;
    }

    void OnDestroy()
    {
        // Show the default cursor when this is destroyed
        Cursor.visible = true;
    }
}