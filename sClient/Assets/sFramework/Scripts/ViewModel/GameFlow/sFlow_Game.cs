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
        
        //sULoading.instance.enableCamera();

        //sPlayerManager.GetInstance().createSelf(1, "test", "xx", 0, startpos);

        
    }

    public override void flowing()
    {
        base.flowing();
        Debug.Log("game flowing");
    }

    public override void flowOut()
    {
        base.flowOut();
        //sULoading.instance.disableCamera();
        
    }

    

   
}
