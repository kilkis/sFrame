using UnityEngine;
using System.Collections;
using sFrame;

public class sFlow_Init : sBaseFlow
{

    public override void flowIn()
    {
        base.flowIn();
        sUpdateGame.GetInstance().checkUpdate();
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
