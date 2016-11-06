using UnityEngine;
using System.Collections;

[RequireComponent(typeof(sGameRoot))]
public class sSingleton<T> : MonoBehaviour where T : sSingleton<T>
{
    private static T _instance;
    public static T GetInstance()
    {
        return _instance;
    }

    public void SetInstance(T t)
    {
        if (_instance == null)
        {
            _instance = t;
        }
    }

    public virtual void Init()
    {
        return;
    }

    public virtual void Release()
    {
        return;
    }
}

