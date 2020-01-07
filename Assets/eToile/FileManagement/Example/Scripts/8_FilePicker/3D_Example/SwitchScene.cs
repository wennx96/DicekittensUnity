﻿using UnityEngine;

public class SwitchScene : MonoBehaviour
{
    Canvas _uiCanvas;
    OpenBrowser _browser;
    Rotate3DObject _rot;

	// Use this for initialization
	void Start ()
    {
        _uiCanvas = GameObject.Find("ExampleUI").GetComponent<Canvas>();
        _rot = gameObject.GetComponent<Rotate3DObject>();
        _browser = gameObject.GetComponent<OpenBrowser>();
    }
	
	public void Show3DScene()
    {
        _rot.Restart();
        _rot.active = true;
        _uiCanvas.enabled = false;
    }

    public void Hide3DScene()
    {
        _rot.Restart();
        _rot.active = false;
        _browser.CloseBrowser();
        _uiCanvas.enabled = true;
    }

}