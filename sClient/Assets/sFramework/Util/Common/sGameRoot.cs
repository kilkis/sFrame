using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using sFramework;

public class sGameRoot : MonoBehaviour
{

    private static GameObject _rootObj;
    private static List<Action> _singletonReleaseList = new List<Action>();

    private float logicTime = 0;

    public void Awake()
    {
        _rootObj = gameObject;
        GameObject.DontDestroyOnLoad(_rootObj);

        InitSingletons();
    }

    void Start()
    {
        sGameFlow.instance.startGame();
    }

    /// <summary>
    /// 在这里进行所有单例的销毁
    /// </summary>
    public void OnApplicationQuit()
    {
        for (int i = _singletonReleaseList.Count - 1; i >= 0; i--)
        {
            _singletonReleaseList[i]();
        }
    }

    /// <summary>
    /// 在这里进行所有单例的初始化
    /// </summary>
    /// <returns></returns>
    private void InitSingletons()
    {
        //ClearCanvas();
        //Debug.Log("init");
        AddSingleton<sEvent>();
        AddSingleton<sLoadAssetbundle>();
        AddSingleton<sCheckMemory>();
        AddSingleton<sCache>();
        AddSingleton<sFirstGame>();//firstGame和UpdateGame的顺序不能变！
        AddSingleton<sUpdateGame>();
        AddSingleton<sLoadingGame>();
        AddSingleton<sLoadBin>();
        AddSingleton<sAvatarMgr>();
        //AddSingleton<sGameFlow>();
        AddSingleton<sMapManager>();
        AddSingleton<sPlayerManager>();
        
        Debug.Log("init singleton over");

    }

    private static void AddSingleton<T>() where T : sSingleton<T>
    {
        if (_rootObj.GetComponent<T>() == null)
        {
            T t = _rootObj.AddComponent<T>();
            t.SetInstance(t);
            t.Init();

            _singletonReleaseList.Add(delegate ()
            {
                t.Release();
            });
        }
    }

    public static T GetSingleton<T>() where T : sSingleton<T>
    {
        T t = _rootObj.GetComponent<T>();

        if (t == null)
        {
            AddSingleton<T>();
        }

        return t;
    }

    public void ClearCanvas()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        foreach (Transform panel in canvas)
        {
            GameObject.Destroy(panel.gameObject);
        }
    }

    void Update()
    {
        if (sAvatarMgr.GetInstance() != null)
        {
            sAvatarMgr.GetInstance().update();
        }


    }

    void FixedUpdate()
    {
        logicTime += Time.deltaTime;
        if( logicTime > sConst.logictInvTime)
        {
            if( sGameFlow.instance != null )
                sGameFlow.instance.logicUpdate();


            logicTime -= sConst.logictInvTime;
        }
    }
}
