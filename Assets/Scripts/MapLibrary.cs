using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;


using EasyMobile;
using SFB;
using Skytanet.SimpleDatabase;

public class MapLibrary
{
    private static SaveFile db = new SaveFile("MapLibrary");

    private static Dictionary<string, Map> maps;

    private static int untitledCounter = 0;

    private static string newlySavedFile = "";

    static MapLibrary()
    {
        untitledCounter = db.Get<int>("MapLibrary-UntitledCounter", 0);
    }

    public static void PickMapImage()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Media.Gallery.Pick(ImportMapEM);
        }
        else
        {
            var extensions = new[]
            {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("All Files", "*" ),
            };
            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, true, ImportMapSFB);
        }
    }

    public static void ImportMapEM(string error, MediaResult[] results)
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

    public static void ImportMapSFB(string[] paths)
    {
        foreach (string path in paths)
        {

        }
    }

    public static void LoadMapImageEM(string error, Texture2D image)
    {
        //FileManagement.SaveJpgTexture()
        var thumbnail = new Texture2D(200, 200);


    }

    public static void LoadMapImageSFB(string path)
    {
        var thumbnail = FileManagement.ImportTexture(path).Resize(200, 200);
    }

    public static void SaveMapImage(Texture2D image)
    {
        var thumbnail = GenerateThumbnail(image, 200, 200);

        //Map newmap = new Map();

    }

    public static Texture2D GenerateThumbnail(Texture2D image, int dstWidth, int dstHeight)
    {
        Texture2D result = new Texture2D(dstWidth, dstHeight, image.format, false);

        float incX = (1.0f / (float)dstWidth);
        float incY = (1.0f / (float)dstHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = image.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);

            }
        }

        result.Apply();
        return result;
    }

    public static string GetFileHash(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        int len = (int)fs.Length;
        byte[] data = new byte[len];
        fs.Read(data, 0, len);
        fs.Close();
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);
        string fileMD5 = "";
        foreach (byte b in result)
        {
            fileMD5 += Convert.ToString(b, 16);
        }
        return fileMD5;
    }


}
