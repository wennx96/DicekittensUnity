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

    public void PickMapImage()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Media.Gallery.Pick(ImportMapEM);
        }
        else
        {
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        }
    }

    public void ImportMapEM(string error, MediaResult[] results)
    {

        foreach (MediaResult result in results)
        {
            // You can use this field to check if the picked item is an image or a video.
            MediaType type = result.Type;
            if (type != MediaType.Image)
            {
                continue;
            }

            // You can use this uri to load the item.
            string uri = result.Uri;
            Media.Gallery.LoadImage(result, LoadMapImageEM);
        }
    }

    public void ImportMapSFB()
    {

    }

    public void LoadMapImageEM(string error, Texture2D image)
    {
        //FileManagement.SaveJpgTexture()
    }

    public void LoadMapImageSFB()
    {

    }
}
