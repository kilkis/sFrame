using UnityEngine;
using System.Collections;
using sFramework;

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
        sULoading.instance.disableCamera();
        scu.obj.SetActive(true);
    
        //sCache.GetInstance().clearScu(scu);
        //scu.obj = null;
        

    }
}
