using UnityEngine;
using System.Collections;

/// <summary>
/// 合并字符串,节省堆内存和优化性能,所有的string的“+”行为必须使用这个
/// </summary>
public class sStringBuilder 
{
    public static string combine(params object[] args)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < args.Length; ++i )
        {
            sb.Append(args[i]);
        }
        return sb.ToString();
    }
}
