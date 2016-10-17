using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using LuaFramework;

public class AssetRef
{
    public int refnum = 0;
    public AssetBundle asset = null;
}

/// <summary>
/// 资源加载机制
/// 1.自动识别读取streamasset还是更新过的资源
/// 2.预先加载总纲和所有shader
/// </summary>
public class sLoadAssetbundle : sSingleton<sLoadAssetbundle> {

    public string m_assetPath = Application.streamingAssetsPath;
    public string m_dataPath = Application.persistentDataPath + "/asset";
    string assetTail = ".sab";

    #region 必须存在于整个生命周期的内容
    private AssetBundleManifest _all = null;//总纲,预先读取并保存
    private List<AssetBundle> _allShaders = null;//所有shader，预先读取并保存
    #endregion

    //资源加载的refrence，用于手动unload所有的资源,自身资源名-ref结构
    private Dictionary<string, AssetRef> _assetRefrence = new Dictionary<string, AssetRef>();

    //每个资源使用的依赖的记录，用于在删除时将依赖也释放,自身资源名-依赖名称list
    private Dictionary<string, List<string>> _depenceRecord = new Dictionary<string, List<string>>();

    //avatar系统的可以删除的资源，除了bone以外，其他部件整合后可以删除
    private List<AssetBundle> _avatarList = new List<AssetBundle>();

    //更新过的文件,如果在这个队列里，就不读取streamasset目录，否则按包内文件处理
    private Dictionary<string, int> _updatedFile = new Dictionary<string, int>();

    public override void Init()
    {
        //这里读取和维护更新后文件的配置
        loadUpdateFile();
    }

    /// <summary>
    /// 读取文件比较信息，获得哪些文件经过了下载更新
    /// </summary>
    private void loadUpdateFile()
    {
        string compFile = sStringBuilder.combine(Application.persistentDataPath, "/updateFile.txt");
        if (File.Exists(compFile))
        {
            //存在文件，读取，并且读入sLoadAssetbundle的updateFile内
            FileStream fs = File.Open(compFile, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string fn = sr.ReadLine();
            while(!string.IsNullOrEmpty(fn))
            {
                string vid = sr.ReadLine();
                //Debug.Log("fn:" + fn);
                //Debug.Log("vid:" + vid);
                pushUpdateFile(fn, int.Parse(vid));

                fn = sr.ReadLine();

            }
            
            sr.Close();
            fs.Close();
        }
        else
        {
            //不存在
        }
    }
    public void pushUpdateFile(string name, int vid)
    {
        //Debug.Log("push update file:" + name + " - " + vid);
        if( _updatedFile.ContainsKey(name))
        {
            _updatedFile[name] = vid;
        }
        else
        {
            _updatedFile.Add(name, vid);
        }
    }

    public void clearUpdateFile()
    {
        _updatedFile.Clear();
    }

    /// <summary>
    /// 将更新文件列表写入硬盘
    /// </summary>
    public void overwriteUpdateFile()
    {
        string compFile = sStringBuilder.combine(Application.persistentDataPath, "/updateFile.txt");
        FileStream fs = File.Open(compFile, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);
        foreach (string name in _updatedFile.Keys)
        {
            sw.WriteLine(name);
            sw.WriteLine(_updatedFile[name].ToString());
        }
        sw.Close();
        fs.Close();
    }

    #region LoadCommonFile
    public void loadCommonFileFromInet(string name, Action<WWW> www)
    {
        StartCoroutine(LoaderResInet(name, www));
    }
    #endregion

    #region LoadAssetBundle
    /// <summary>
    /// 加载AssetBundleManifest,会在进入更新完毕后加载,并且整个游戏生命周期内不进行任何卸载
    /// </summary>
    public void LoadAssetBundleManifest()
    {
        if (_all != null)
            return;
        Debug.Log("########## load manifest");
        string manifestName = this.GetRuntimePlatform();
        //manifestName = manifestName + "/" + manifestName;//eg:Windows/Windows
        manifestName = "StreamingAssets";
        LoadResReturnWWW(manifestName, (www) =>
        {
            AssetBundle assetBundle = www.assetBundle;
            UnityEngine.Object obj = assetBundle.LoadAsset("AssetBundleManifest");
            assetBundle.Unload(false);
            AssetBundleManifest manif = obj as AssetBundleManifest;
            _all = manif;

            www.Dispose();
            www = null;
            Debug.Log("game load manifest");
            //shawn-在game读取了manifest后，赋值到lua内
            LuaFramework.ResourceManager.instance.Initialize(_all);
            loadAllShader();
        });
    }
    /// <summary>
    /// 加载所有的shader，会在总纲加载完成后加载,并且整个游戏生命周期内不进行任何卸载
    /// </summary>
    private void loadAllShader()
    {
        if (_allShaders != null)
            return;
        if (_all == null)
            return;
        
        _allShaders = new List<AssetBundle>();
        string[] tmp = _all.GetAllAssetBundles();
        int finishedCount = 0;
        int alllen = 0;
        for (int i = 0; i < tmp.Length; ++i)
        {
            if (tmp[i].EndsWith(".s"))
            {
                ++alllen;
            }
        }
        Debug.Log("start load all shader : "+alllen);
        for ( int i = 0; i < tmp.Length; ++i )
        {
            if( tmp[i].EndsWith(".s"))
            {
                Debug.LogWarning("load shader:" + tmp[i]);
                string realName = tmp[i];//eg:Windows/ui/panel.unity3d
                
                LoadResReturnWWW(realName, (www) =>
                {
                    if (www != null)
                    {
                        int index = realName.LastIndexOf("/");
                        string assetName = realName.Substring(index + 1);
                        assetName = assetName.Replace(assetTail, "");

                        AssetBundle assetBundle = www.assetBundle;
                        //shader常驻，不需要删除，load即可
                        if (assetBundle != null)
                            assetBundle.LoadAllAssets();
                        //_allShaders.Add(www.assetBundle);
                        
                        finishedCount++;
                        //Debug.Log("finishedCount:" + finishedCount + "/" + alllen);
                        if (finishedCount == alllen)
                        {
                            //所有shader加载完成
                            Debug.Log("all shader load ok, now warmup them!");
                            Shader.WarmupAllShaders();
                            Debug.Log("warmup ok!");

                            sEvent.GetInstance().callEvent(UpdateEvent.LoadAllFinish.ToString());
                        }
                        //不手动清空，会保留96B数据
                        www.Dispose();
                        www = null;
                    }
                });
            }
        }
        tmp = null;

        
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="name"></param>
    public void unloadAssetBundle(string name)
    {
        name = sStringBuilder.combine(name, assetTail);
        if (_depenceRecord.ContainsKey(name))
        {
            for (int i = 0; i < _depenceRecord[name].Count; ++i)
            {
                if (_depenceRecord[name][i].EndsWith(".s"))
                    continue;
                --_assetRefrence[_depenceRecord[name][i]].refnum;
                if (_assetRefrence[_depenceRecord[name][i]].refnum == 0)
                {
                    _assetRefrence[_depenceRecord[name][i]].asset.Unload(true);
                    _assetRefrence[_depenceRecord[name][i]].asset = null;
                    _assetRefrence.Remove(_depenceRecord[name][i]);
                }
            }
        }
        --_assetRefrence[name].refnum;
        if( _assetRefrence[name].refnum == 0 )
        {
            if (_assetRefrence[name].asset)
            {
                _assetRefrence[name].asset.Unload(true);
                _assetRefrence[name].asset = null;
            }
            _assetRefrence.Remove(name);
        }
    }

    public void unloadAssetBundleForce(string name)
    {
        if(_assetRefrence.ContainsKey(name))
        {
            _assetRefrence[name].asset.Unload(true);
            _assetRefrence.Remove(name);
        }
    }

    public void unloadAvatarTemp()
    {
        for(int i = 0;i < _avatarList.Count; ++i )
        {
            if (_avatarList[i] != null)
            {
                _avatarList[i].Unload(true);
                _avatarList[i] = null;
            }
        }
        _avatarList.Clear();
    }

    public void loadNavmesh(string name, Action callback)
    {
        if (string.IsNullOrEmpty(name))
            return;
        name = sStringBuilder.combine(name, assetTail);

        string realName = name;
        if (_assetRefrence.ContainsKey(name))
        {
            //navmesh加载过了就不会再次加载
            callback();
        }
        else
        {
            LoadResReturnWWW(realName, (www) =>
            {
                int index = realName.LastIndexOf("/");
                string assetName = realName.Substring(index + 1);
                assetName = assetName.Replace(assetTail, "");
                AssetBundle assetBundle = www.assetBundle;
                _assetRefrence.Add(name, new AssetRef());
                _assetRefrence[name].asset = assetBundle;
                ++_assetRefrence[name].refnum;
                callback();
            });
        }
    }

    public void unloadNavmesh(string name)
    {
        name = sStringBuilder.combine(name, assetTail);
        --_assetRefrence[name].refnum;
        if (_assetRefrence[name].refnum == 0)
        {
            if (_assetRefrence[name].asset)
            {
                _assetRefrence[name].asset.Unload(true);
                _assetRefrence[name].asset = null;
            }
            _assetRefrence.Remove(name);
        }
    }

    public void loadAssetBundle(string name, Action<UnityEngine.Object> callback, bool isAvatar = false)
    {
        if (string.IsNullOrEmpty(name))
            return;
        name = sStringBuilder.combine(name, assetTail);
        //Debug.Log("start load:" + name + " ###############################");
        Action action = () =>
        {
            string realName = name;//eg:Windows/ui/panel.unity3d
            //已经加载过了
            if( _assetRefrence.ContainsKey(name))
            {

                int index = realName.LastIndexOf("/");
                string assetName = realName.Substring(index + 1);
                assetName = assetName.Replace(assetTail, "");
                //Debug.Log("asset:" + assetName + ": "+ _assetRefrence[name].asset);
                UnityEngine.Object obj = _assetRefrence[name].asset.LoadAsset(assetName);
                //Debug.Log("obj:" + obj);
                callback(obj);

                ++_assetRefrence[name].refnum;
            }
            else
            LoadResReturnWWW(realName, (www) =>
            {
                int index = realName.LastIndexOf("/");
                string assetName = realName.Substring(index + 1);
                assetName = assetName.Replace(assetTail, "");
                AssetBundle assetBundle = www.assetBundle;
                //Debug.Log("asset:" + assetName);
                UnityEngine.Object obj = assetBundle.LoadAsset(assetName);//LoadAsset(name）,这个name没有后缀,eg:panel
                if (!isAvatar)
                {
                    if (_assetRefrence.ContainsKey(name))
                    {
                        ++_assetRefrence[name].refnum;
                    }
                    else
                    {
                        _assetRefrence.Add(name, new AssetRef());
                        _assetRefrence[name].asset = assetBundle;
                        ++_assetRefrence[name].refnum;
                    }
                }
                else
                {
                    _avatarList.Add(assetBundle);
                }
                //加载目标资源完成的回调
                callback(obj);
                
                //卸载资源内存
                //assetBundle.Unload(false);//这个不能清除！！！！要不然list里的保存会被清除,赋值不算使用
                assetBundle = null;

                //Debug.Log("_assetRefrence[name].asset:" + _assetRefrence[name].asset);
                //不手动清空，会保留96B数据
                //www.Dispose();
                //www = null;
            });
 
        };
 
        LoadDependenceAssets(name, action, isAvatar);
    }
    private void LoadDependenceAssets(string targetAssetName, Action action, bool isAvatar)
    {
        if (targetAssetName.Contains("_bone"))
        {
            action();
            return;
        }
        //Debug.Log("要加载的目标资源:" + targetAssetName);//ui/panel.unity3d
        
        string[] dependences = _all.GetAllDependencies(targetAssetName);
        //Debug.Log("依赖文件个数：" + dependences.Length);
        int length = dependences.Length;
        int finishedCount = 0;
        if (length == 0)
        {
            //没有依赖
            action();
        }
        else
        {
            if (!isAvatar)
            {
                if (!_depenceRecord.ContainsKey(targetAssetName))
                    _depenceRecord.Add(targetAssetName, new List<string>());
            }
            //有依赖，加载所有依赖资源
            for (int i = length - 1; i >= 0; i--)
            {
                string dependenceAssetName = dependences[i];
                
                dependenceAssetName = dependenceAssetName;//eg:Windows/altas/heroiconatlas.unity3d

                //shader不需要再次加载了
                if (dependenceAssetName.EndsWith(".s"))
                {
                    ++finishedCount;
                    if (finishedCount == length)
                    {
                        //依赖都加载完了
                        action();
                    }
                }
                //已经加载过了
                else
                {
                    if (!isAvatar)
                    {
                        if (!_depenceRecord[targetAssetName].Contains(dependenceAssetName))
                            _depenceRecord[targetAssetName].Add(dependenceAssetName);
                    }
                    //判定是否是加载了还没有删除的asset
                    if (_assetRefrence.ContainsKey(dependenceAssetName))
                    {
                        ++_assetRefrence[dependenceAssetName].refnum;
                        ++finishedCount;
                        if (finishedCount == length)
                        {
                            //依赖都加载完了
                            action();
                        }
                    }
                    else
                    {
                        LoadResReturnWWW(dependenceAssetName, (www) =>
                        {
                            if (www != null)
                            {
                                //int index = dependenceAssetName.LastIndexOf("/");
                                //string assetName = dependenceAssetName.Substring(index + 1);
                                //assetName = assetName.Replace(assetTail, "");
                                AssetBundle assetbundle = www.assetBundle;
                                if (!isAvatar)
                                {
                                    //Debug.Log("add ar:" + dependenceAssetName);
                                    if (_assetRefrence.ContainsKey(dependenceAssetName))
                                    {
                                        ++_assetRefrence[dependenceAssetName].refnum;
                                    }
                                    else
                                    {
                                        _assetRefrence.Add(dependenceAssetName, new AssetRef());
                                        _assetRefrence[dependenceAssetName].asset = assetbundle;
                                        ++_assetRefrence[dependenceAssetName].refnum;
                                    }
                                }
                                else
                                {
                                    _avatarList.Add(assetbundle);
                                }
                                ++finishedCount;

                                if (finishedCount == length)
                                {
                                    //依赖都加载完了
                                    action();
                                }
                                
                            }
                        });
                    }
                }
            }
        }



    }


    #endregion

    #region ExcuteLoader
    public void LoadResReturnWWW(string name, Action<WWW> callback)
    {
        //Debug.Log("加载：" + name);
        if (_cbs.ContainsKey(name))
            _cbs[name].Add(callback);
        else
        {
            _cbs.Add(name, new List<Action<WWW>>());
            _cbs[name].Add(callback);
            StartCoroutine(LoaderRes(name));
        }
    }

    IEnumerator LoaderResInet(string name, Action<WWW> callback)
    {
        WWW www = new WWW(name);
        
        //Debug.Log("load res inet:" + path);

        yield return www;
        if (www.isDone)
        {
            if (www.error == null)
                callback(www);
            else
                callback(null);
        }
    }

    private Dictionary<string, List<Action<WWW>>> _cbs = new Dictionary<string, List<Action<WWW>>>();

    IEnumerator LoaderRes(string name)
    {
        //Debug.Log("laod res:" + name);
        
        WWW www = null;
        string path = "";
        //这里需要根据下载后的文件以及比较码，进行分类处理
        if (_updatedFile.ContainsKey(name))
        {
            path = sStringBuilder.combine("file://", this.m_dataPath, "/", name);
            www = WWW.LoadFromCacheOrDownload(path, _updatedFile[name]);
        }
        else
        {
            path = sStringBuilder.combine("file://", this.m_assetPath, "/", name);
            www = new WWW(path);
        }
        
        
        //Debug.Log("load res:" + path);
        
        yield return www;
        if (www.isDone)
        {
            for(int i = 0; i <  _cbs[name].Count; ++i )
            {
                _cbs[name][i](www);
            }
            _cbs.Remove(name);
        }
        //不手动清空，会保留96B数据
        www.Dispose();
        www = null;
    }
    #endregion
 
    #region Util
    /// <summary>
    /// 平台对应文件夹
    /// </summary>
    /// <returns></returns>
    private string GetRuntimePlatform()
    {
        string platform = "";
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            platform = "Windows";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            platform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            platform = "IOS";
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            platform = "OSX";
        }
        return platform;
    }

    #endregion
}


