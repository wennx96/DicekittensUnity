using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindowResize : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform window;                                // The window intended to be resized.
    public int minWidth = 400;
    public int minHeight = 300;
    RectTransform rootCanvas;                                   // The container of this window (reference calculations).
    Vector2 pointerOffset;                                      // First clic coordinates.
    Vector2 pointerPosition;                                    // Current pointer coordiantes.

    void Start()
    {
        if (window == null)
            Debug.LogError("[UIWindowResize] " + transform.name + " hasn't a main window RectTransform assigned.");
        else
            rootCanvas = window.parent.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        // Pointer clic in canvas coordinates:
        RectTransformUtility.ScreenPointToLocalPointInRectangle(window, data.position, data.pressEventCamera, out pointerOffset);
        Vector3[] corners = new Vector3[4];
        window.GetLocalCorners(corners);
        pointerOffset = (Vector2)corners[3] - pointerOffset;
    }

    public void OnDrag(PointerEventData data)
    {
        // Pointer drag in canvas coordinates:
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas, data.position, data.pressEventCamera, out pointerPosition))
        {
            Vector2 offsetMax = window.offsetMax;
            Vector2 offsetMin = window.offsetMin;
            // Apply the new size:
            Vector2 delta = pointerPosition + pointerOffset;
            window.offsetMax = new Vector2(delta.x, window.offsetMax.y);     // Upper right corner.
            window.offsetMin = new Vector2(window.offsetMin.x, delta.y);     // Lower left corner.
            // Check for minimum size limits:
            Rect size = window.rect;
            if (size.width < minWidth)
                window.offsetMax = offsetMax;
            if (size.height < minHeight)
                window.offsetMin = offsetMin;
        }
    }
}
