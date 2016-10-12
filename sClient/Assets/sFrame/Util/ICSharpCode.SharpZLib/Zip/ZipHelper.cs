using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using UnityEngine;
using System.Threading;

/// <summary>
/// Zip压缩与解压缩 
/// </summary>
public class ZipHelper
{
    public delegate void LoadCallbackHandle(int progress, int progressMax, string desc);
    public static event LoadCallbackHandle LoadCB;

    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="fileToZip">要压缩的文件</param>
    /// <param name="zipedFile">压缩后的文件</param>
    /// <param name="compressionLevel">压缩等级</param>
    /// <param name="blockSize">每次写入大小</param>
    public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
    {
        //如果文件没有找到，则报错
        if (!System.IO.File.Exists(fileToZip))
        {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        }

        using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
        {
            using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
            {
                using (System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);

                    ZipEntry ZipEntry = new ZipEntry(fileName);

                    ZipStream.PutNextEntry(ZipEntry);

                    ZipStream.SetLevel(compressionLevel);

                    byte[] buffer = new byte[blockSize];

                    int sizeRead = 0;

                    try
                    {
                        do
                        {
                            sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                            ZipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }

                    StreamToZip.Close();
                }

                ZipStream.Finish();
                ZipStream.Close();
            }

            ZipFile.Close();
        }
    }

    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="fileToZip">要进行压缩的文件名</param>
    /// <param name="zipedFile">压缩后生成的压缩文件名</param>
    public static void ZipFile(string fileToZip, string zipedFile)
    {
        //如果文件没有找到，则报错
        if (!File.Exists(fileToZip))
        {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        }

        using (FileStream fs = File.OpenRead(fileToZip))
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            using (FileStream ZipFile = File.Create(zipedFile))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(5);

                    ZipStream.Write(buffer, 0, buffer.Length);
                    ZipStream.Finish();
                    ZipStream.Close();
                }
            }
        }
    }

    public static void StartZipDir(string DirToZip, string ZipedFile, int CompressionLevel)
    {
        ZipDir(DirToZip, ZipedFile, CompressionLevel);
    }

    /// <summary>
    /// 压缩文件夹的方法
    /// </summary>
    public static void ZipDir(string DirToZip, string ZipedFile, int CompressionLevel)
    {
        string path = "";
        //压缩文件为空时默认与压缩文件夹同一级目录
        if (ZipedFile == string.Empty)
        {
            ZipedFile = DirToZip.Substring(DirToZip.LastIndexOf("/") + 1);
            ZipedFile = DirToZip.Substring(0, DirToZip.LastIndexOf("/")) + "/" + ZipedFile + ".zip";
        }
        Dictionary<string, DateTime> fileList = getAllFies(DirToZip);
        int cur = 0;
        path = ZipedFile;
        while (cur < fileList.Count)
        {
            ////Debug.Log(path);
            using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(path)))
            {
                zipoutputstream.SetLevel(CompressionLevel);
                Crc32 crc = new Crc32();
                List<string> keys = new List<string>(fileList.Keys);
                for (int i = cur; i < keys.Count; ++i)
                //foreach (DictionaryEntry item in fileList)
                {
                    DictionaryEntry item = new DictionaryEntry(keys[i], fileList[keys[i]]);
                    FileStream fs = File.OpenRead(item.Key.ToString());
                    //Debug.Log(fs.Name);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ////Debug.Log(item.Key.ToString() + ":" + buffer.Length);
                    ZipEntry entry = new ZipEntry(item.Key.ToString().Substring(DirToZip.Length + 1));
                    entry.DateTime = (DateTime)item.Value;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipoutputstream.PutNextEntry(entry);
                    zipoutputstream.Write(buffer, 0, buffer.Length);

                    //fileList.Remove(item);
                    ++cur;
                    if (LoadCB != null)
                    {
                        LoadCB(cur, fileList.Count, "update.zip" + " 压缩中......");
                    }
                }
            }
            //File.Move(string.Concat(Application.persistentDataPath + "/" + "update.zip"), string.Concat(Application.dataPath + "/sUpdateResource"));
        }
        //FileStream filestream = new FileStream(Application.streamingAssetsPath + "/zip", FileMode.CreateNew);
        //StreamWriter sw = new StreamWriter(filestream);
        //sw.Write(bao);
        //sw.Close();
        //filestream.Close();
    }

    /// <summary>
    /// 获取所有文件
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, DateTime> getAllFies(string dir)
    {
        //Hashtable FilesList = new Hashtable();
        Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
        DirectoryInfo fileDire = new DirectoryInfo(dir);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }

        getAllDirFiles(fileDire, FilesList);
        getAllDirsFiles(fileDire.GetDirectories(), FilesList);
        return FilesList;
    }

    /// <summary>
    /// 获取一个文件夹下的所有文件夹里的文件
    /// </summary>
    /// <param name="dirs"></param>
    /// <param name="filesList"></param>
    private static void getAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
    {
        foreach (DirectoryInfo dir in dirs)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                if (file.FullName.EndsWith(".meta"))
                    continue;
                filesList.Add(file.FullName, file.LastWriteTime);
            }
            getAllDirsFiles(dir.GetDirectories(), filesList);
        }
    }

    /// <summary>
    /// 获取一个文件夹下的文件
    /// </summary>
    /// <param name="strDirName">目录名称</param>
    /// <param name="filesList">文件列表HastTable</param>
    private static void getAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
    {
        foreach (FileInfo file in dir.GetFiles("*.*"))
        {
            if (file.FullName.EndsWith(".meta") || file.FullName.EndsWith(".svn"))
                continue;
            filesList.Add(file.FullName, file.LastWriteTime);
        }
    }
}
