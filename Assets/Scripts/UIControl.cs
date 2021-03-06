﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using EasyMobile;
using SFB;


public class UIControl : MonoBehaviour
{
    List<GameObject> UICanvases = new List<GameObject>();
    List<GraphicRaycaster> UIGraphicRaycasters = new List<GraphicRaycaster>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            UICanvases.Add(go);
            UIGraphicRaycasters.Add(go.GetComponent<GraphicRaycaster>());
            if (go.name.Contains("Toolbar"))
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public (string, List<RaycastResult>) GetRayHitOnUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();

        string uiName = "";

        foreach (GraphicRaycaster canvas in UIGraphicRaycasters)
        {
            if (!canvas.gameObject.activeSelf) continue;
            canvas.Raycast(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                uiName = canvas.gameObject.name;
                break;
            }
        }

        return (uiName, results);
    }

    public void ShowUI(GameObject ui)
    {
        foreach (GameObject uiCanvas in UICanvases)
        {
            uiCanvas.SetActive(uiCanvas == ui);
        }
    }

    public void BackToGame()
    {
        foreach (GameObject uiCanvas in UICanvases)
        {
            uiCanvas.SetActive(uiCanvas.name.Contains("Toolbar"));
        }
    }

    public void ImportMap()
    {
        MapLibrary.PickMapImage();
    }
}
