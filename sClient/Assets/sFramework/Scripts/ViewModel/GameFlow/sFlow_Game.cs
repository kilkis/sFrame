using UnityEngine;
using System.Collections;
using sFramework;
using UnityEngine.SceneManagement;

public class sFlow_Game : sBaseFlow
{
    public Vector3 startpos = new Vector3(0.1f, 0.0f, -9.3f);

    GameObject player;

    public override void flowIn()
    {
        base.flowIn();
        sLoadingGame.GetInstance().loadWeak("scene/newplayerscene1", _loadcallback, false);//生成A
        sLoadingGame.GetInstance().loadNavmesh("newplayerscene1", _loadnavcallback);//生成C-navmesh根本，有了它才能loadscene
        //sULoading.instance.enableCamera();

        //sPlayerManager.GetInstance().createSelf(1, "test", "xx", 0, startpos);
    }

    public override void flowing()
    {
        base.flowing();
    }

    public override void flowOut()
    {
        base.flowOut();
        //sULoading.instance.disableCamera();
        sCache.GetInstance().clearCache("scene/newplayerscene1");//删除A

        SceneManager.UnloadScene("newplayerscene1");//删除B-navmesh scene load
        sLoadingGame.GetInstance().unloadNavmesh("newplayerscene1");//删除C
    }

    public void _loadnavcallback()
    {
        SceneManager.LoadSceneAsync("newplayerscene1", LoadSceneMode.Additive);//生成B-navmesh scene load
        
    }

    public void _loadcallback(sCacheUnit scu)
    {
        //Debug.Log("loadcb:" + scu);
        scu.obj.SetActive(true);
        sULoading.instance.hideLoading();
        //sCache.GetInstance().clearScu(scu);
        //scu.obj = null;
        
        
        

        
    }
}
