using UnityEngine;
using System.Collections;

public class sFlow_Login : sBaseFlow {

    public override void flowIn()
    {
        base.flowIn();


        LuaFramework.Util.CallMethod("NoticeCtrl", "Awake");
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
