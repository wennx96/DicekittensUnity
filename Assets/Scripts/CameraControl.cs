using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //General Objs
    private Camera Cam;
    private GameObject Map;
    private GameObject MapPlane;
    private Vector3 CurrentRotation;
    private Vector3 LastMousePosition;

    private Vector3 LastTouch0Position;
    private Vector3 LastTouch1Position;




    private Ray GetCursorRay() => Cam.ScreenPointToRay(Input.mousePosition);
    private Ray GetMiddleRay() => Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    private bool IsMouseLeftHold() => Input.GetMouseButton(0) && !IsTouching();
    private bool IsMouseMiddleHold() => Input.GetMouseButton(2) && !IsTouching();

    private bool IsTouching() => TouchCount() > 0;
    private int TouchCount() => Input.touchCount;
    private Vector3 GetTouch(int index) => Input.GetTouch(index).position;
    private bool IsTouchDown(int index) => Input.GetTouch(index).phase == TouchPhase.Began;
    private bool IsTouchMove(int index) =>
        Input.GetTouch(index).phase == TouchPhase.Moved
        || Input.GetTouch(index).phase == TouchPhase.Stationary;
    private bool IsTouchUp(int index) =>
        Input.GetTouch(index).phase == TouchPhase.Ended
        || Input.GetTouch(index).phase == TouchPhase.Canceled;


    string GUIText1 = "";
    string GUIText2 = "";
    string GUIText3 = "";
    string GUIText4 = "";

    private Vector3 GetTouchDifference() => GetTouch(0) - GetTouch(1);

    //private Ray GetRay()=>Cam.ScreenPointToRay(IsTouching?GetTouch0():Input.GetAxis())

    void OnGUI()
    {
        GUI.Label(new Rect(60, 160, 200, 50), GUIText1);
        GUI.Label(new Rect(60, 260, 200, 50), GUIText2);
        GUI.Label(new Rect(60, 360, 200, 50), GUIText3);
        GUI.Label(new Rect(60, 460, 200, 50), GUIText4);

        //GUI.Box(new Rect(GetTouch(0).x, Screen.height - GetTouch(0).y, 40, 40), "0");
        //GUI.Box(new Rect(GetTouch(1).x, Screen.height - GetTouch(1).y, 40, 40), "1");
    }

    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
        Map = GameObject.Find("Map");
        MapPlane = Map.transform.Find("MapPlane").gameObject;
        CurrentRotation = transform.eulerAngles;
        LastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTouching() && IsTouchDown(0)) LastTouch0Position = GetTouch(0);
        if (TouchCount() == 2 && IsTouchDown(1)) LastTouch1Position = GetTouch(1);
        GUIText1 = TouchCount() > 0 ? Input.GetTouch(0).phase.ToString() + " " + GetTouch(0).ToString() : "No Touch 1";
        GUIText2 = TouchCount() > 1 ? Input.GetTouch(0).phase.ToString() + " " + GetTouch(1).ToString() : "No Touch 2";
        GUIText4 = Input.mousePosition.ToString();
        if (IsTouching()) GUIText4 = $"{IsTouchDown(0)}-{IsTouchMove(0)}-{IsTouchUp(0)}";
        if (Drag()) return;
        if (Hold()) return;
        if (Drop()) return;
        if (Move()) return;
        if (Rotate()) return;
        if (Zoom()) return;

    }

    void LateUpdate()
    {
        LastMousePosition = Input.mousePosition;
        if (IsTouching()) LastTouch0Position = GetTouch(0);
        if (TouchCount() == 2) LastTouch1Position = GetTouch(1);
    }

    //Drag and Drop Objects
    private bool DraggingState = false;
    private GameObject DraggingObject;
    private GameObject TracingBase;
    private Vector3 ScreenSpace;
    private Vector3 DraggingOffset;
    Vector3 PreDraggingObjectPosition;
    Vector3 DroppingOffset;

    private bool Drag()
    {
        if (!IsTouching())
        {
            if (!Input.GetKeyDown(0)) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchDown(0)) return false;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(GetCursorRay(), out hitInfo))
        {
            DraggingObject = hitInfo.collider.gameObject;
            if (DraggingObject.transform.parent != null)
            {
                Debug.Log("Drag");
                ScreenSpace = Cam.WorldToScreenPoint(DraggingObject.transform.parent.position);
                DraggingOffset = new Vector3(
                    0, 0, 0);//Obj.transform.parent.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,screenSpace.z));
                PreDraggingObjectPosition = DraggingObject.transform.parent.position;
            }
            else
            {
                return false;
            }
            DraggingState = DraggingObject.GetComponent("Immovable") != null;
            return DraggingState;
        }
        return true;
    }

    private bool Hold()
    {
        if (!DraggingState) return false;
        if (!IsMouseLeftHold())
        {
            return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchMove(0)) return false;
        }
        Debug.Log("Hold");
        float x, y, z;
        float t;

        y = DraggingObject.transform.parent.position.y;

        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z);
        Vector3 currentPosition = Cam.ScreenToWorldPoint(currentScreenSpace) + DraggingOffset;

        t = (y - Cam.transform.position.y) / (currentPosition.y - Cam.transform.position.y);

        x = Cam.transform.position.x - t * (Cam.transform.position.x - currentPosition.x);
        z = Cam.transform.position.z - t * (Cam.transform.position.z - currentPosition.z);
        currentPosition = new Vector3(x, y, z);
        DraggingObject.transform.parent.position = currentPosition;

        //Create a tracing base on grid;

        return true;
    }

    private bool Drop()
    {
        if (!DraggingState) return false;
        if (!IsTouching())
        {
            if (!Input.GetMouseButtonUp(0)) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchUp(0)) return false;
        }
        Debug.Log("Drop");
        float x = DraggingObject.transform.parent.position.x;
        float y = DraggingObject.transform.parent.position.y;
        float z = DraggingObject.transform.parent.position.z;

        DroppingOffset = DraggingObject.transform.parent.position - PreDraggingObjectPosition;

        //Snap to grid

        DraggingObject.transform.parent.transform.position = new Vector3(x, 0.3f, z);//SquarePosition;

        //Destroy the tracing base

        DraggingState = false;

        return true;
    }

    //Move
    private readonly float MoveSpeed = 0.5f;
    Vector3 CursorDelta;

    private bool Move()
    {
        if (DraggingState) return false;
        if (!IsTouching())
        {
            if (!IsMouseLeftHold()) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchMove(0)) return false;
        }
        Debug.Log("Move");
        Vector3 oldPosition;
        Vector3 newPosition;
        if (IsMouseLeftHold())
        {
            CursorDelta = Input.mousePosition - LastMousePosition;
            oldPosition = LastMousePosition;
            newPosition = Input.mousePosition;
        }
        else
        {
            CursorDelta = GetTouch(0) - LastTouch0Position;
            oldPosition = LastTouch0Position;
            newPosition = GetTouch(0);
        }

        Ray oldRay = Cam.ScreenPointToRay(oldPosition);
        Ray newRay = Cam.ScreenPointToRay(newPosition);

        Vector3 oldPoint;
        Vector3 newPoint;

        RaycastHit oldHitInfo;
        RaycastHit newHitInfo;

        if (!Physics.Raycast(oldRay, out oldHitInfo, float.MaxValue, 1 << 10)) return true;
        oldPoint = oldRay.GetPoint(oldHitInfo.distance);

        if (!Physics.Raycast(newRay, out newHitInfo, float.MaxValue, 1 << 10)) return true;
        newPoint = newRay.GetPoint(newHitInfo.distance);

        CursorDelta = newPoint - oldPoint;
        transform.parent.Translate(-CursorDelta.x, -CursorDelta.y, -CursorDelta.z);

        //if (IsTouching() && CursorDelta.magnitude > 50) return true;

        //transform.Translate(-CursorDelta * MoveSpeed * Time.deltaTime);

        return true;
    }


    private Vector3 TouchCenter;


    private readonly int RotateSpeed = 4;
    private float LastTouchAngle;

    private bool Rotate()
    {
        if (!IsTouching())
        {
            if (!IsMouseMiddleHold()) return false;
            Debug.Log("Rotate");
            CurrentRotation.x += CursorDelta.y * RotateSpeed * Time.deltaTime;
            CurrentRotation.y -= CursorDelta.x * RotateSpeed * Time.deltaTime;
            transform.eulerAngles = CurrentRotation;
        }
        else if (TouchCount() == 2)
        {
            Debug.Log("Rotate");
            float angle = Vector3.Angle(GetTouch(0), GetTouch(1));
            Ray midray = GetMiddleRay();


        }



        return true;
    }

    private float LastTouchDistance;

    private readonly float ZoomSpeed = 2;

    private bool Zoom()
    {
        float MouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (Math.Abs(MouseScroll) <= 1E-3) return false;
        transform.parent.Translate(0, 0, MouseScroll * ZoomSpeed);
        return true;
    }

}
