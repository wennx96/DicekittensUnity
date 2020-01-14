using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;


using EasyMobile;
using SFB;
using Skytanet.SimpleDatabase;

public class DebugHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void File_GetSavedFiles()
    {
        AssetLibrary al = new AssetLibrary();
        MapLibrary ml = new MapLibrary();
        Debug.Log("File_GetSavedFiles");
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        foreach (FileInfo file in di.GetFiles("*.*", SearchOption.AllDirectories))
        {
            Debug.Log(file.FullName);
        }
    }

    public void File_ComputeHash()
    {
        Debug.Log(Asset.ComputeMD5(Application.persistentDataPath + "/MapLibrary.save"));
        Debug.Log(Asset.ComputeMD5(Application.persistentDataPath + "/MapLibrary.save").Length);

    }
}
