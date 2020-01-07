using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using EasyMobile;

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

    public void PickMapImage()
    {
        Media.Gallery.Pick(ImportMap);
    }

    public void ImportMap(string error, MediaResult[] results)
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
        }
    }
}
