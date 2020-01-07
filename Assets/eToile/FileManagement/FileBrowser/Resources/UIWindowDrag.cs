using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Generic script to drag a window on 2D and 3D environments:
 * 1 - Attach this script to the visual element intended to handle the window when dragged.
 * 2 - Assigtn the main virual element that works as the widow itself (can be other than "handle" gameObject).
 */

public class UIWindowDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform window;                // The window intended to be dragged.
    public bool clampToCanvas = true;           // Clamp the windows inside the container canvas.
    RectTransform rootCanvas;                   // The container of this window (reference calculations).
    Vector2 pointerOffset;

    void Start()
    {
        if (window == null)
            Debug.LogError("[UIWindowDrag] " + transform.name + " hasn't a main window RectTransform assigned.");
        else
            rootCanvas = window.parent.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        window.SetAsLastSibling();              // Brings to front but under the same canvas.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(window, data.position, data.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas, data.position, data.pressEventCamera, out localPointerPosition))
        {
            if(clampToCanvas)
                window.localPosition = ClampToWindow(localPointerPosition) - pointerOffset;
            else
                window.localPosition = localPointerPosition - pointerOffset;
        }
    }

    Vector2 ClampToWindow(Vector2 data)
    {
        Vector3[] canvasCorners = new Vector3[4];
        rootCanvas.GetLocalCorners(canvasCorners);

        float clampedX = Mathf.Clamp(data.x, canvasCorners[0].x, canvasCorners[2].x);
        float clampedY = Mathf.Clamp(data.y, canvasCorners[0].y, canvasCorners[2].y);

        return new Vector2(clampedX, clampedY);
    }
}