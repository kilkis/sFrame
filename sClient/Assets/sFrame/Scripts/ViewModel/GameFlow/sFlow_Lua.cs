using UnityEngine;
using System.Collections;
using sFrame;

public class sFlow_Lua : sBaseFlow
{

    public override void flowIn()
    {
        base.flowIn();
        sULoading.instance.showLoading();

        AppFacade.Instance.StartUp();   //lua初始化
    }

    public override void flowing()
    {
        base.flowing();
    }

    public override void flowOut()
    {
        base.flowOut();
    }
}
