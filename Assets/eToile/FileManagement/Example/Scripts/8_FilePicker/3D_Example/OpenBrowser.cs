using UnityEngine;

/*
 * This script is attached to rhe 3D cube in the example scene.
 */

public class OpenBrowser : MonoBehaviour
{
    // Browser parameters:
    public GameObject browser;
    public Transform parent;
    GameObject instance;

    // Opens a new file frowser:
    public void Open()
    {
        if(instance == null)
        {
            instance = Instantiate(browser);
            instance.transform.SetParent(parent, false);
        }
    }
    
    // Hide the 3D example:
    public void CloseBrowser()
    {
        if (instance != null)
            GameObject.Destroy(instance);
    }
}
