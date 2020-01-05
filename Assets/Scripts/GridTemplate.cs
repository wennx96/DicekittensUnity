using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTemplate : MonoBehaviour
{
    bool DraggingState = false;

    Vector3 Offset;

    private bool IsTouchDown(int index) => Input.touchCount > index && Input.GetTouch(index).phase == TouchPhase.Began;
    private bool IsTouchMove(int index) =>
        Input.touchCount > index && (Input.GetTouch(index).phase == TouchPhase.Moved
        || Input.GetTouch(index).phase == TouchPhase.Stationary);
    private bool IsTouchUp(int index) =>
        Input.touchCount > index && (Input.GetTouch(index).phase == TouchPhase.Ended
        || Input.GetTouch(index).phase == TouchPhase.Canceled);


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || IsTouchDown(0))
        {
            Offset = transform.position - Input.mousePosition;
            DraggingState = true;
            return;
        }
        if (DraggingState)
        {
            transform.position = Input.mousePosition + Offset;
        }
        if (Input.GetMouseButtonUp(0) || IsTouchUp(0))
        {
            DraggingState = false;
        }
    }


}
