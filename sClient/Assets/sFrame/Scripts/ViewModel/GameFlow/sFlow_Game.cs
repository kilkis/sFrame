using UnityEngine;
using System.Collections;
using sFrame;
using UnityEngine.SceneManagement;

public class sFlow_Game : sBaseFlow
{
    public Vector3 startpos = new Vector3(0.1f, 0.0f, -9.3f);

    GameObject player;

    public override void flowIn()
    {
        base.flowIn();
        sLoadingGame.GetInstance().loadWeak("scene/testscene", _loadcallback, false);
        sLoadingGame.GetInstance().loadNavmesh("navTest", _loadnavcallback);
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
        sCache.GetInstance().clearCache("scene/testscene");

        SceneManager.UnloadScene("testscene");
        sLoadingGame.GetInstance().unloadNavmesh("navTest");
    }

    public void _loadnavcallback()
    {
        SceneManager.LoadSceneAsync("testscene", LoadSceneMode.Additive);
        
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
