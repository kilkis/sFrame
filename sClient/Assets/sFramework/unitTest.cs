using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using sFramework;
using UnityEngine.SceneManagement;

public class unitTest : MonoBehaviour {

    GameObject go;

    void Awake()
    {
        
    }
	// Use this for initialization
	void Start () {
        //sFramework.sULoading.instance.showLoading();

        sCheckMemory.GetInstance().checkGCMemory();
        
        sGameFlow.instance.startGame();
        
	}

    public void _loadcallback(sCacheUnit scu)
    {
        //Debug.Log("loadcb:" + scu);
        scu.obj.SetActive(true);
        tsc.Add(scu);
        //sCache.GetInstance().clearScu(scu);
        //scu.obj = null;


    }
	
	// Update is called once per frame
	void Update () {
	    
	}
    List<sCacheUnit> tsc = new List<sCacheUnit>();
    AssetBundle ab;

    GameObject go1 = null;
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 50, 0, 50, 50), "test"))
        {
            for (int i = 0; i < tsc.Count; ++i)
            {
                sLoadingGame.GetInstance().unloadWeak(tsc[i]);
            }

            //for (int i = 0; i < 30; ++i)
            //  sAvatarMgr.GetInstance().deletePlayer(i+1);

            //sCache.GetInstance().clearScu(tsc);
            //Debug.Log("scu:" + tsc);
            //tsc = null;

            //Destroy(go1);
            //go1 = null;
            //ab.Unload(true);
            //ab = null;
            //sLoadingGame.GetInstance().loadWeak("testdragon", _loadcallback, true);
            //sLoadingGame.GetInstance().loadWeak("testdragon", _loadcallback, false);
            //sAvatarMgr.GetInstance().createPlayer(1, "avatar/testavatar_bone", "testavatar_wp_004", new string[] { "avatar/testavatar_004_tou", "avatar/testavatar_004_shou", "avatar/testavatar_004_shen", "avatar/testavatar_004_jiao" });

            sCheckMemory.GetInstance().checkGCMemory();
        }
        if (GUI.Button(new Rect(Screen.width - 100, 0, 50, 50), "test1"))
        {
            //sLoadingGame.GetInstance().loadWeak("ch_pc_hou_004_tou", _loadcallback, false);
            //Resources.UnloadUnusedAssets();
            //sAvatarMgr.GetInstance().deletePlayer(1);
            //sAvatarMgr.GetInstance().createPlayer(2, "avatar/testavatar_bone", "avatar/testavatar_wp_004", new string[] { "avatar/testavatar_004_tou", "avatar/testavatar_004_shou", "avatar/testavatar_004_shen", "avatar/testavatar_004_jiao" });
            for (int i = 0; i < 1; ++i)
            {
                //sAvatarMgr.GetInstance().createPlayer(i + 1, "FS_bone", "", new string[] { "FS_chest_000", "FS_foot_000", "FS_hand_000", "FS_head_000", "FS_leg_000" });
            }

            //StartCoroutine(load());
            sCheckMemory.GetInstance().checkGCMemory();
        }
        if (GUI.Button(new Rect(Screen.width - 150, 0, 50, 50), "test2"))
        {
            sLoadingGame.GetInstance().loadWeak("cube", _loadcallback, false);
            sLoadingGame.GetInstance().loadWeak("testdragon", _loadcallback, false);

            sCheckMemory.GetInstance().checkGCMemory();
        }
        if (GUI.Button(new Rect(Screen.width - 200, 0, 50, 50), "shake"))
        {
            //sCamera.instance.shake();
            //卸载场景，先卸载scene，后去除navmesh
            SceneManager.UnloadScene("testscene");
            sLoadingGame.GetInstance().unloadNavmesh("navTest");
        }
    }

    IEnumerator load()
    {
        string path = "file://"+Application.streamingAssetsPath + "/sAssetBundle/Windows/ch_pc_hou_004_tou.sab";
        WWW www = new WWW(path);
        while (!www.isDone)
            yield return null;

        ab = www.assetBundle;
        if( ab != null )
        {
            UnityEngine.Object obj = ab.LoadAsset("ch_pc_hou_004_tou");
            go1 = GameObject.Instantiate(obj) as GameObject;
            
        }
        
        
        www.Dispose();
        www = null;

        

        
    }

    public void onUpdateState(int state, string vid)
    {
        Debug.Log("onUpdateState: " + state + " -- " + vid);
        

    }

    public void onUpdateFinish()
    {
        Debug.Log("onUpdateFinish");
        
    }

    #region 测试2种字符合并的消耗
    public void testString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int i = 0;
        long StartTime = DateTime.Now.Ticks;
        while (i < 100000)
        {
            sb.Append(i.ToString());
            i++;
        }
        long EndTime = DateTime.Now.Ticks;

        Debug.Log("时间:" + (EndTime - StartTime));//600034

        string sb1 = null;
        i = 0;
        StartTime = DateTime.Now.Ticks;
        while (i < 100000)
        {
            sb1 += i;
            i++;
        }
        EndTime = DateTime.Now.Ticks;
        Debug.Log("时间:" + (EndTime - StartTime));//966135260
    }
    #endregion
}
