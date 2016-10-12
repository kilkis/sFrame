using UnityEngine;
using System.Collections;

/// <summary>
/// 检测内存相关机制
/// </summary>
public class sCheckMemory : sSingleton<sCheckMemory>
{

    public void checkSystemMemory()
    {
        Debug.Log("系统总内存：" + SystemInfo.systemMemorySize + "M");
        //return SystemInfo.systemMemorySize > 900;
    }

    public void checkGCMemory()
    {
        float gcmemory = (float)System.GC.GetTotalMemory(true) * 0.00000095f;
        Debug.Log("堆内存:" + gcmemory.ToString() + "MB");
    }
}
