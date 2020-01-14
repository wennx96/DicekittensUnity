using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Asset
{
    string Name;
    int ReferenceID;
    string FilePath;
    string FileHash;
    long FileSize;
    DateTime CreateTime;
    DateTime LastModifyTime;

    public Asset(string path, string name = "", int id = 0)
    {
        FileInfo fi = new FileInfo(path);
        Name = name;
        FilePath = path;
        FileHash = ComputeMD5(path);
        FileSize = fi.Length;
        if (AssetLibrary.ReferenceCounter.ContainsKey(FileHash))
        {
            ReferenceID = AssetLibrary.ReferenceCounter[FileHash]++;
        }
        else
        {
            ReferenceID = 0;
            AssetLibrary.ReferenceCounter.Add(FileHash, 1);
        }
        CreateTime = DateTime.Now;
        LastModifyTime = DateTime.Now;
    }

    public byte[] GetAllBytes()
    {
        return File.ReadAllBytes(FilePath);
        foreach (var b in GetBytes(10))
        {

        }
    }

    public IEnumerable<byte[]> GetBytes(int size)
    {
        for (long i = 0; i < FileSize; i += size)
        {
            byte[] bytes = new byte[size];
            File.Open(FilePath, FileMode.Open, FileAccess.Read).Read(bytes, (int)i, size);
            yield return bytes;
        }
    }



    public static String ComputeMD5(String fileName)
    {
        String hashMD5 = String.Empty;
        //检查文件是否存在，如果文件存在则进行计算，否则返回空值
        if (System.IO.File.Exists(fileName))
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //计算文件的MD5值
                System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                Byte[] buffer = calculator.ComputeHash(fs);
                calculator.Clear();
                //将字节数组转换成十六进制的字符串形式
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    stringBuilder.Append(buffer[i].ToString("x2"));
                }
                hashMD5 = stringBuilder.ToString();
            }//关闭文件流
        }//结束计算
        return hashMD5;
    }
}
