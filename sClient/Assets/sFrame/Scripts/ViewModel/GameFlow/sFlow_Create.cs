using UnityEngine;
using System.Collections;
using sFrame;

public class sFlow_Create : sBaseFlow
{

    public override void flowIn()
    {
        base.flowIn();
        sLoadingGame.GetInstance().loadWeak("scene/createscene", _loadcallback, false);
        //sULoading.instance.enableCamera();
    }

    public override void flowing()
    {
        base.flowing();
    }

    public override void flowOut()
    {
        base.flowOut();
        //sULoading.instance.disableCamera();
        sCache.GetInstance().clearCache("scene/createscene");

    }

    public void _loadcallback(sCacheUnit scu)
    {
        //Debug.Log("loadcb:" + scu);
        scu.obj.SetActive(true);
    
        //sCache.GetInstance().clearScu(scu);
        //scu.obj = null;
        

    }
}
