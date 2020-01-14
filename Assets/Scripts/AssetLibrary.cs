using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;


using EasyMobile;
using SFB;
using Skytanet.SimpleDatabase;

public class AssetLibrary
{
    private static SaveFile db = new SaveFile("AssetLibrary");

    public static Dictionary<string, Asset> Assets { get; set; } = new Dictionary<string, Asset>();

    public static Dictionary<string, int> ReferenceCounter { get; set; } = new Dictionary<string, int>();

    public static int UntitledCounter { get; set; } = 0;

    static AssetLibrary()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/AssetLibrary"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/AssetLibrary");
        }
    }

    public static void ImportAsset(string path)
    {
        FileInfo fi = new FileInfo(path);
        File.Copy(path, Application.persistentDataPath + "/" + fi.Name);
    }

    public static void ImportAsset(string filename, byte[] data)
    {

    }

    public Asset GetAsset()
    {
        return default(Asset);
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
