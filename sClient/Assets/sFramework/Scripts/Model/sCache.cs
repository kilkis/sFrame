using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CacheType
{
    single,
    avatar,
    particle,
}

public class sCacheUnit
{
    public string name = "";
    public GameObject obj = null;

    public bool isUsing = false;
}
/// <summary>
/// 缓存文件里读取的prefab，并用作之后的'源'
/// 可以设置每个cache的obj的默认数量，当实际需求超过数量，则会扩充最大数量
/// 这个cache机制只用于cache小物件，类似角色，特效等，地形需要有专属定制cache
/// </summary>
public class sCache : sSingleton<sCache>
{
    private Dictionary<string, List<sCacheUnit>> _caches = new Dictionary<string, List<sCacheUnit>>();

    //底层cache，之后的cache根据这个来
    private Dictionary<string, GameObject> _deepCache = new Dictionary<string, GameObject>();
    public void initCache(CacheType ctype, string name, UnityEngine.Object go, bool noCache)
    {
        if( _caches.ContainsKey(name))
        {
            Debug.LogError("same name cache:" + name);
            return;
        }
        int cachenum = 1;
        if (ctype == CacheType.avatar)
            cachenum = 2;
        else if (ctype == CacheType.particle)
            cachenum = 2;
        _caches.Add(name, new List<sCacheUnit>());
        //go.SetActive(false);
        if (noCache )
        {
            //Debug.Log("nocache: "+name);
            sCacheUnit cu = new sCacheUnit();
            cu.name = name;
            cu.obj = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
            cu.isUsing = false;

            _caches[name].Add(cu);
            return;
        }
        _deepCache.Add(name, GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject);
        _deepCache[name].SetActive(false);

        for(int i = 0;i < cachenum; ++i )
        {
            sCacheUnit cu = new sCacheUnit();
            cu.name = name;
            cu.obj = GameObject.Instantiate(_deepCache[name], Vector3.zero, Quaternion.identity) as GameObject;
            cu.isUsing = false;
            
            _caches[name].Add(cu);
        }
    }

    public sCacheUnit getUnusedCache(string name)
    {
        if( _caches.ContainsKey(name))
        {
            if (!_deepCache.ContainsKey(name))
                return _caches[name][0];
            for(int i = 0;i < _caches[name].Count; ++i )
            {
                if (_caches[name][i].isUsing)
                    continue;
                return _caches[name][i];
            }
            int num = _caches[name].Count;
            //数量全部被用完，需要手动扩充
            for(int i = 0; i < 2; ++i )
            {
                sCacheUnit cu = new sCacheUnit();
            
                cu.obj = GameObject.Instantiate(_deepCache[name], Vector3.zero, Quaternion.identity) as GameObject;
                cu.isUsing = false;
                _caches[name].Add(cu);
            }
            return _caches[name][num];
        }
        return null;
    }

    /// <summary>
    /// 不是真实的卸载，而是disable
    /// </summary>
    /// <param name="scu"></param>
    public void clearScu(sCacheUnit scu)
    {
        scu.isUsing = false;
        scu.obj.SetActive(false);
    }

    /// <summary>
    /// 真实卸载，内存中全部去除
    /// </summary>
    /// <param name="name"></param>
    public void clearCache(string name)
    {
        if( _caches.ContainsKey(name))
        {
            for(int i = 0;i < _caches[name].Count; ++i )
            {
                GameObject.Destroy(_caches[name][i].obj);
                sLoadAssetbundle.GetInstance().unloadAssetBundle(name);
            }

            _caches[name].Clear();
            _caches.Remove(name);
        }
    }

    /// <summary>
    /// loading换场景时调用
    /// </summary>
    public void clearAllCache()
    {

    }
}
