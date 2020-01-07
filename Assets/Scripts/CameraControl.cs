using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    //General Objs

    public bool DisableCameraControl;
    private Camera Cam;
    private GameObject Map;
    private GameObject MapPlane;
    private Vector3 CurrentRotation;
    private Vector3 LastMousePosition;
    private UIControl UIControl;

    private Vector3 LastTouch0Position;
    private Vector3 LastTouch1Position;

    private float LastTouchDistance;
    private float LastTouchAngle;
    private Vector3 LastTouchMidpoint;


    private Ray GetCursorRay() => Cam.ScreenPointToRay(Input.mousePosition);
    private Ray GetMiddleRay() => Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

    private bool IsMouseLeftDown() => Input.GetMouseButtonDown(0) && !IsTouching();
    private bool IsMouseLeftHold() => Input.GetMouseButton(0) && !IsTouching();
    private bool IsMouseLeftUp() => Input.GetMouseButtonUp(0) && !IsTouching();
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
        UIControl = GameObject.Find("MasterCanvas").GetComponent<UIControl>();
        CurrentRotation = transform.eulerAngles;
        LastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (DisableCameraControl) return;
        var (uiCanvas, raycastResult) = UIControl.GetRayHitOnUI();

        if (uiCanvas.Contains("View") && !uiCanvas.Contains("Toolbar")) return;

        if (IsTouching() && IsTouchDown(0)) LastTouch0Position = GetTouch(0);
        if (TouchCount() == 2 && IsTouchDown(1)) LastTouch1Position = GetTouch(1);
        if (IsTouching())
        {
            if (IsTouchDown(0))
            {
                LastTouch0Position = GetTouch(0);
            }
            if (TouchCount() == 2)
            {
                if (IsTouchDown(1))
                {
                    LastTouch1Position = GetTouch(1);
                    LastTouchDistance = Vector3.Distance(GetTouch(0), GetTouch(1));
                    LastTouchMidpoint = (GetTouch(0) + GetTouch(1)) / 2;
                    LastTouchAngle = Vector3.Angle(GetTouch(0), GetTouch(1));
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (LastMousePosition.x < 0 || LastMousePosition.x > Screen.width || LastMousePosition.y < 0 || LastMousePosition.y > Screen.height)
                    LastMousePosition = Input.mousePosition;
            }
        }
        GUIText1 = TouchCount() > 0 ? Input.GetTouch(0).phase.ToString() + " " + GetTouch(0).ToString() : "No Touch 1";
        GUIText2 = TouchCount() > 1 ? Input.GetTouch(0).phase.ToString() + " " + GetTouch(1).ToString() : "No Touch 2";
        GUIText3 = DraggingState.ToString();
        GUIText4 = TouchCount().ToString();
        //if (IsTouching()) GUIText4 = $"{IsTouchDown(0)}-{IsTouchMove(0)}-{IsTouchUp(0)}";
        // Debug.Log("=====");
        if (Drag()) return;
        //Debug.Log("Tried Drag");
        if (Hold()) return;
        //Debug.Log("Tried Hold");
        if (Drop()) return;
        //Debug.Log("Tried Drop");
        if (Rotate()) ;
        //Debug.Log("Tried Rotate");
        if (Zoom()) ;
        //Debug.Log("Tried Zoom");
        if (Move()) ;

    }

    void LateUpdate()
    {
        LastMousePosition = Input.mousePosition;
        if (IsTouching()) LastTouch0Position = GetTouch(0);
        if (TouchCount() == 2)
        {
            LastTouch1Position = GetTouch(1);
            LastTouchDistance = Vector3.Distance(GetTouch(0), GetTouch(1));
            LastTouchAngle = Vector3.Angle(GetTouch(0), GetTouch(1));
            LastTouchMidpoint = (GetTouch(0) + GetTouch(1)) / 2;
        }
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
        if (DraggingState) return false;
        if (!IsTouching())
        {
            if (!IsMouseLeftDown()) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchDown(0)) return false;
        }
        else return false;

        RaycastHit hitInfo;
        if (Physics.Raycast(GetCursorRay(), out hitInfo, float.MaxValue, 1 << 13))
        {
            DraggingObject = hitInfo.collider.gameObject;
            if (DraggingObject != null)
            {
                ScreenSpace = Cam.WorldToScreenPoint(DraggingObject.transform.position);
                DraggingOffset = /*Vector3.zero;*/DraggingObject.transform.position - Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z));
                PreDraggingObjectPosition = DraggingObject.transform.position;
                DraggingObject.GetComponent<Pawn>().Lifted = true;
            }
            else
            {
                return false;
            }
            DraggingState = DraggingObject.transform.gameObject.GetComponent("Immovable") == null;
            return DraggingState;
        }
        return true;
    }

    private bool Hold()
    {
        if (!DraggingState) return false;
        if (!IsTouching())
        {
            if (!IsMouseLeftHold()) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchMove(0)) return false;
        }
        else return false;
        float x, y, z;
        float t;

        y = DraggingObject.transform.position.y;

        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z);
        Vector3 currentPosition = Cam.ScreenToWorldPoint(currentScreenSpace) + DraggingOffset;

        t = (y - Cam.transform.position.y) / (currentPosition.y - Cam.transform.position.y);

        x = Cam.transform.position.x - t * (Cam.transform.position.x - currentPosition.x);
        z = Cam.transform.position.z - t * (Cam.transform.position.z - currentPosition.z);
        currentPosition = new Vector3(x, y, z);
        DraggingObject.transform.position = currentPosition;

        //Create a tracing base on grid;

        return true;
    }

    private bool Drop()
    {
        if (!DraggingState) return false;
        if (!IsTouching())
        {
            if (!IsMouseLeftUp()) return false;
        }
        else if (TouchCount() == 1)
        {
            if (!IsTouchUp(0)) return false;
        }
        else return false;
        float x = DraggingObject.transform.position.x;
        float y = DraggingObject.transform.position.y;
        float z = DraggingObject.transform.position.z;

        DroppingOffset = DraggingObject.transform.position - PreDraggingObjectPosition;

        //Snap to grid

        DraggingObject.transform.transform.position = new Vector3(x, 0f, z);//SquarePosition;
        DraggingObject.GetComponent<Pawn>().Lifted = false;

        //Destroy the tracing base

        DraggingState = false;

        return true;
    }

    //Move
    private readonly float MoveSpeed = 0.5f;
    Vector3 CursorDelta;

    int MoveFingerID;

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
            if (Input.GetTouch(0).fingerId != MoveFingerID)
            {
                MoveFingerID = Input.GetTouch(0).fingerId;
                return false;
            }
        }
        else return false;
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

        if (!Physics.Raycast(oldRay, out RaycastHit oldHitInfo, float.MaxValue, 1 << 10)) return true;
        oldPoint = oldRay.GetPoint(oldHitInfo.distance);

        if (!Physics.Raycast(newRay, out RaycastHit newHitInfo, float.MaxValue, 1 << 10)) return true;
        newPoint = newRay.GetPoint(newHitInfo.distance);

        CursorDelta = newPoint - oldPoint;
        transform.parent.position -= CursorDelta;

        //if (IsTouching() && CursorDelta.magnitude > 50) return true;

        //transform.Translate(-CursorDelta * MoveSpeed * Time.deltaTime);
        if (IsTouching()) MoveFingerID = Input.GetTouch(0).fingerId;

        return true;
    }


    private Vector3 TouchCenter;


    private readonly int RotateSpeed = 4;

    private bool Rotate()
    {
        if (!IsTouching())
        {
            if (!IsMouseMiddleHold()) return false;
            CursorDelta = Input.mousePosition - LastMousePosition;
            CurrentRotation.x += CursorDelta.y * RotateSpeed * Time.deltaTime;
            CurrentRotation.y -= CursorDelta.x * RotateSpeed * Time.deltaTime;
            transform.eulerAngles = CurrentRotation;
        }
        else if (TouchCount() == 2)
        {
            float angle = Vector3.Angle(GetTouch(0), GetTouch(1));
            float deltaAngle = angle - LastTouchAngle;
            deltaAngle = Vector3.SignedAngle(LastTouch1Position - LastTouch0Position, GetTouch(1) - GetTouch(0), Vector3.forward);
            if (Mathf.Abs(deltaAngle) < 0.5)
            {
                Vector3 midpoint = (GetTouch(0) + GetTouch(1)) / 2;
                float delta = midpoint.y - LastTouchMidpoint.y;
                Cam.transform.Rotate(delta / 50, 0f, 0f);
            }
            else
            {
                Cam.transform.parent.Rotate(0f, deltaAngle, 0f);
            }
            Ray midray = GetMiddleRay();


        }
        else return false;


        return true;
    }


    private readonly float ZoomSpeed = 2;

    private Vector3 ZoomCenter;

    private bool Zoom()
    {
        float MouseScroll = Input.GetAxis("Mouse ScrollWheel");
        float delta = 0;
        Vector3 targetPosition;
        if (TouchCount() != 2)
        {
            if (Math.Abs(MouseScroll) <= 1E-3)
            {
                return false;
            }
            targetPosition = Input.mousePosition;
            delta = MouseScroll * ZoomSpeed;
            Ray direction = Cam.ScreenPointToRay(targetPosition);
            if (!Physics.Raycast(direction, out RaycastHit hitInfo, float.MaxValue, 1 << 10)) return true;
            Vector3 targetPoint = direction.GetPoint(hitInfo.distance);
            Vector3 zoomVector = targetPoint - Cam.transform.position;
            zoomVector.Normalize();
            transform.parent.Translate(-zoomVector * delta);
        }
        else
        {

            if (IsTouchDown(0) || IsTouchDown(1))
            {
                ZoomCenter = (GetTouch(0) + GetTouch(1)) / 2;
                Ray ray = Cam.ScreenPointToRay(ZoomCenter);
                if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 10)) return true;
                ZoomCenter = ray.GetPoint(hit.distance);
            }

            float distance = Vector3.Distance(GetTouch(0), GetTouch(1));
            float touchChangeMultiplier = distance / LastTouchDistance;


            //Vector3 focalPoint = ray.GetPoint(hit.distance);
            Vector3 focalPoint = ZoomCenter;
            //Vector3 focalPoint = Vector3.zero;
            //Vector3 focalPoint = Cam.transform.parent.position + Cam.transform.forward;
            Vector3 direction = Cam.transform.parent.position - ZoomCenter;
            float newDistance = direction.magnitude / touchChangeMultiplier;
            Cam.transform.parent.position = newDistance * direction.normalized + ZoomCenter;
            Debug.DrawRay(Cam.transform.parent.position, ZoomCenter);

            //Fixed rate zooming
            /*float distance = Vector3.Distance(GetTouch(0), GetTouch(1));
            delta = distance - LastTouchDistance;
            targetPosition = (GetTouch(0) + GetTouch(1)) / 2;
            LastTouchDistance = distance;
            Debug.Log("Entering Zoom");
            if (Math.Abs(delta) < 2) return false;

            Vector3 zoomVector = Cam.ScreenPointToRay(targetPosition).direction;

            Debug.Log(zoomVector);
            transform.parent.Translate(zoomVector * delta / 20 * ZoomSpeed);*/


            //Raybased testing scripts
            /*Ray ray0 = Cam.ScreenPointToRay(LastTouch0Position);
            Ray ray1 = Cam.ScreenPointToRay(LastTouch1Position);

            Debug.Log($"Casting Ray to {LastTouch0Position} and {LastTouch1Position}");

            Debug.Log("Start Raycasting");
            if (!Physics.Raycast(ray0, out RaycastHit hitInfo0, float.MaxValue, 1 << 10)) return true;
            Debug.Log($"Ray0 {ray0} Hit! Hitpoint: {ray0.GetPoint(hitInfo0.distance)} Distance: {hitInfo0.distance}");
            if (!Physics.Raycast(ray1, out RaycastHit hitInfo1, float.MaxValue, 1 << 10)) return true;
            Debug.Log($"Ray1 {ray1} Hit! Hitpoint: {ray1.GetPoint(hitInfo1.distance)} Distance: {hitInfo1.distance}");
            Vector3 mapPoint0 = ray0.GetPoint(hitInfo0.distance);
            Vector3 mapPoint1 = ray1.GetPoint(hitInfo1.distance);

            Vector3 screenPoint0 = GetTouch(0);
            Vector3 screenPoint1 = GetTouch(1);

            screenPoint0.z = Cam.nearClipPlane;
            screenPoint1.z = Cam.nearClipPlane;

            Vector3 worldPoint0 = Cam.ScreenToWorldPoint(screenPoint0);
            Vector3 worldPoint1 = Cam.ScreenToWorldPoint(screenPoint1);

            Debug.Log($"Transform screen point 0 {screenPoint0} to {worldPoint0}");
            Debug.Log($"Transform screen point 1 {screenPoint1} to {worldPoint1}");

            if (ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, mapPoint0, screenPoint0 - mapPoint0, mapPoint1, screenPoint1 - mapPoint1)) return true;
            Vector3 newCamPosition = (closestPointLine1 + closestPointLine2) / 2;
            Vector3 camMovement = newCamPosition - transform.parent.position;

            Debug.Log($"Calculated transform: {transform.parent.position} => {newCamPosition}");

            transform.parent.Translate(camMovement);*/

        }
        /*Ray direction = Cam.ScreenPointToRay(targetPosition);
        if (!Physics.Raycast(direction, out RaycastHit hitInfo, float.MaxValue, 1 << 10)) return true;
        Vector3 targetPoint = direction.GetPoint(hitInfo.distance);
        Vector3 zoomVector = targetPoint - Cam.transform.position;
        zoomVector.Normalize();
        Debug.Log(zoomVector);
        transform.parent.Translate(-zoomVector * delta);*/
        return true;
    }


}
