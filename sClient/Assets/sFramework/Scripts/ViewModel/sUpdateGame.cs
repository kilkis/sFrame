using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;

//todo:更新完毕后删除zip文件
public enum UpdateEvent
{
    NeedUpdateState,//检测更新后的状态回调 -- int[state], string[version]
    UpdatingState,//正在更新中的状态回调 -- int[state], string[filename], int[percent]
    UpdateFinish,//更新完成回调 -- #
    LoadAllFinish,//基础资源加载完成
}
/// <summary>
/// 游戏更新机制
/// 1.网上获取版本号，比较更新
/// 2.解压更新包，更新内外资源表(用于确认读取方式，内：随包stream目录下，外：更新下来的文件）
/// </summary>
public class sUpdateGame : sSingleton<sUpdateGame>
{
    private bool unZipFinish = false;
    private long ZipLen = 0;

    int newversion_1 = 0;
    int newversion_2 = 0;
    int newversion_3 = 0;
    public override void Init()
    {
        
    }    
   
    /// <summary>
    /// 检查是否需要更新
    /// </summary>
    /// <returns></returns>
    public void checkUpdate()
    {
        //临时改为无需update
        sLoadAssetbundle.GetInstance().LoadAssetBundleManifest();
        sEvent.GetInstance().callEvent(UpdateEvent.UpdateFinish.ToString());
        return;
        string wwwFile = sStringBuilder.combine(sConst.updateURL, "version.txt");

        Debug.Log("load version file:" + wwwFile);
        sLoadAssetbundle.GetInstance().loadCommonFileFromInet(wwwFile, (www) =>
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("download version file error! " + www.error);
            }
            else
            {
                string ver = www.text;
                Debug.Log("new version:" + ver);
                string[] vers = ver.Split('.');
                if (vers.Length != 3)
                {
                    Debug.LogError("server version file error");
                }
                else
                {
                    newversion_1 = int.Parse(vers[0]);
                    newversion_2 = int.Parse(vers[1]);
                    newversion_3 = int.Parse(vers[2]);
                    int _state = 0;
                    if (newversion_1 != sConst.version_1)//大版本不一致，必须去平台更新，这里不执行更新
                    {
                        _state = 1;
                    }
                    else if (newversion_2 != sConst.version_2)//中版本不一致，不考虑之前小版本，从0开始，进行中版本更新
                    {
                        _state = 2;
                    }
                    else if (newversion_3 != sConst.version_3)//小版本不一致，
                    {
                        _state = 3;
                    }
                    else//版本最新，无需更新
                    {
                        Debug.Log("version is newest");
                        sEvent.GetInstance().callEvent(UpdateEvent.UpdateFinish.ToString());
                    }

                    //通知vm层，有版本需要更新
                    if (_state > 0)
                        sEvent.GetInstance().callEvent(UpdateEvent.NeedUpdateState.ToString(), _state, sStringBuilder.combine(newversion_1, ".", newversion_2, ".", newversion_3));
                    else
                        //todo:目前这个是异步的，不是顺序的,需要改为可控回调
                        sLoadAssetbundle.GetInstance().LoadAssetBundleManifest();
                    //test:测试下载
                    //startDownload();
                }

            }

            www.Dispose();
            www = null;
        });
    }


    /// <summary>
    /// 确认更新后开始下载
    /// </summary>
    public void startDownload()
    {
        if (newversion_1 == 0)//确认得到过版本文件
            return;
        if( newversion_1 > sConst.version_1 )//大版本不一致，必须去平台更新，这里不执行更新
        {

        }
        else if (newversion_2 > sConst.version_2)//中版本不一致，不考虑之前小版本，从0开始，进行中版本更新【未测试】
        {
            string filename = sStringBuilder.combine(newversion_1, ".", newversion_2, ".0.zip");
            string filepath = sStringBuilder.combine(sConst.updateURL, filename);
            sEvent.GetInstance().callEvent(UpdateEvent.UpdatingState.ToString(), 1, filename, 0);
            sLoadAssetbundle.GetInstance().loadCommonFileFromInet(filepath, (www) =>
                {
                    if( www.error != null )
                    {
                        //下载失败,需要通知V
                        Debug.Log("download "+filename+" failed");
                        return;
                    }
                    else
                    {
                        string tmpname = sStringBuilder.combine(Application.persistentDataPath, "/tmpzip/", filename);
                        Debug.Log("tmpname2:" + tmpname);
                        File.WriteAllBytes(tmpname, www.bytes);
                        www.Dispose();
                        www = null;
                        
                        //在中版本不一致的情况下，需要清除之前旧的更新文件
                        sLoadAssetbundle.GetInstance().clearUpdateFile();
                        
                        UnZip(tmpname, sStringBuilder.combine(Application.persistentDataPath, "/asset/"), "", true, newversion_1*100+newversion_2*10+newversion_3);
                        sLoadAssetbundle.GetInstance().overwriteUpdateFile();

                        sConst.version_2 = newversion_2;
                        _remarkVersionFile();
                        startDownload();
                    }
                });
        }
        else if (newversion_3 > sConst.version_3)//小版本不一致，
        {
            string filename = sStringBuilder.combine(newversion_1, ".", newversion_2, ".", sConst.version_3 + 1, ".zip");
            string filepath = sStringBuilder.combine(sConst.updateURL, filename);
            sEvent.GetInstance().callEvent(UpdateEvent.UpdatingState.ToString(), 1, filename, 0);
            sLoadAssetbundle.GetInstance().loadCommonFileFromInet(filepath, (www) =>
                {
                    if (www.error != null)
                    {
                        //下载失败,需要通知V
                        Debug.Log("download " + filepath + " failed");
                        return;
                    }
                    else
                    {
                        string tmpname = sStringBuilder.combine(Application.persistentDataPath, "/tmpzip/", filename);
                        Debug.Log("tmpname3:" + tmpname);
                        File.WriteAllBytes(tmpname, www.bytes);
                        www.Dispose();
                        www = null;

                        UnZip(tmpname, sStringBuilder.combine(Application.persistentDataPath, "/asset/"), "", true, newversion_1 * 100 + newversion_2 * 10 + newversion_3);
                        sLoadAssetbundle.GetInstance().overwriteUpdateFile();

                        ++sConst.version_3;
                        _remarkVersionFile();
                        startDownload();
                    }
                });
        }
        else//更新完成
        {
            //todo:目前这个是异步的，不是顺序的,需要改为可控回调
            sLoadAssetbundle.GetInstance().LoadAssetBundleManifest();
            sEvent.GetInstance().callEvent(UpdateEvent.UpdateFinish.ToString());
        }
    }

    private void _remarkVersionFile()
    {
        string versionFile = sStringBuilder.combine(Application.persistentDataPath, "/version.txt");
        FileStream fs = File.Open(versionFile, FileMode.Open);
        StreamWriter bw = new StreamWriter(fs);
        bw.Write( sStringBuilder.combine(newversion_1, ".", newversion_2, ".", sConst.version_3) );
        bw.Flush();
        bw.Close();
        bw.Dispose();
        bw = null;
        fs.Close();
        fs.Dispose();
        fs = null;
    }

    /*
     * //先下载文件
     * string zipPath = Const.WebUrl + this.ZIPName;
            www = new WWW(zipPath); yield return www;
            if (www.error != null)
            {
                OnUpdateFailed(this.ZIPName);   //
                yield break;
            }
            File.WriteAllBytes(dataPath + Const.AppName, www.bytes);
     * //开线程解压
     * this.thread = new Thread(this.UnZip);
            this.thread.Start();
            while (!this.unZipFinish)
            {
                yield return new WaitForEndOfFrame();
            }
            this.unZipFinish = false;
     * 
     * 
     * UnZip(this.zipName + Const.AppName, this.zipName, "", true);
     */

    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipedFile">The ziped file.</param>
    /// <param name="strDirectory">The STR directory.</param>
    /// <param name="password">zip 文件的密码。</param>
    /// <param name="overWrite">是否覆盖已存在的文件。</param>
    public void UnZip(string zipedFile, string strDirectory, string password, bool overWrite, int vid)
    {
        this.unZipFinish = false;
        this.ZipLen = 0;
        if (strDirectory == "")
            strDirectory = Directory.GetCurrentDirectory();
        //if (!strDirectory.EndsWith("\\"))
        //    strDirectory = strDirectory + "\\";
        ////Debug.Log(strDirectory);
        sEvent.GetInstance().callEvent(UpdateEvent.UpdatingState.ToString(), 2, zipedFile, 0);//开始解压回调
        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
        {
            s.Password = password;
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = "";
                string pathToZip = "";
                pathToZip = theEntry.Name;
                pathToZip = pathToZip.Replace("\\", "/");
                string fileName = Path.GetFileName(pathToZip);
                directoryName = pathToZip.Replace(fileName, "");
                Directory.CreateDirectory(strDirectory + directoryName);
                ////Debug.Log(strDirectory + directoryName + fileName);
                if (fileName != "")
                {
                    if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                    {
                        using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                        {
                            int size = 8192;
                            byte[] data = new byte[size];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                this.ZipLen += size;
                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }
                            data = new byte[0];
                            streamWriter.Close();
                            sLoadAssetbundle.GetInstance().pushUpdateFile(fileName, vid);
                        }
                    }
                }
                Thread.Sleep(0);
            }
            s.Close();
        }
        this.unZipFinish = true;
    }
}
