using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 游戏Loading的类
/// 1.换场景时的强Loading
/// 2.游戏进行时的弱loading
/// </summary>
public class sLoadingGame : sSingleton<sLoadingGame> {

    //读取回调，将cache结构返回，所有操作都需要针对cache
    public delegate void LoadCallback(sCacheUnit scu);
    public delegate void LoadAvatarCallback(UnityEngine.Object obj);
    public delegate void LoadNavCallback();

    //变换场景时需要加载的所有文件
    private List<string> _forceLoadingList = new List<string>();
    //游戏过程中正在加载的文件，如果加载过的，就会进入cache
    private Dictionary<string, object> _weakLoadingList = new Dictionary<string,object>();
    
    public void loadNavmesh(string name, LoadNavCallback _callback)
    {
        sLoadAssetbundle.GetInstance().loadNavmesh(name, () =>
        {
            if (_callback != null)
                _callback();
        });
    }

    public void unloadNavmesh(string name)
    {
        sLoadAssetbundle.GetInstance().unloadNavmesh(name);
    }
    /// <summary>
    /// 在游戏过程中的加载【需要预防：加载完后回调已经被删除】
    /// 可以用于loading过程，也能用于游戏过程，可以选择是否缓存
    /// </summary>
    /// <param name="name"></param>
    /// <param name="_callback"></param>
    /// <param name="noCache"></param>
    public void loadWeak(string name, LoadCallback _callback, bool cache)
    {
        //确认cache
        sCacheUnit scu = sCache.GetInstance().getUnusedCache(name);
        if( scu != null )
        {
            scu.isUsing = true;
            if (_callback != null)
            {
                if (sCache.GetInstance().isLoadOK(name))
                    _callback(scu);
                else
                    sCache.GetInstance().pushCache(name, _callback);
            }
            return;
        }
        //如果cache无法解决，则读取资源(需要先创建一个scu，用于确保下次读取时不用重复加载）
        sCache.GetInstance().initPreCache(CacheType.single, name, !cache);
        sCache.GetInstance().pushCache(name, _callback);
        sLoadAssetbundle.GetInstance().loadAssetBundle(name, (obj) =>
            {
                sCache.GetInstance().cacheLoadOK(name, obj);
            });
    }


    /// <summary>
    /// 这个不是卸载，只是将scu通过cache隐藏,等待其他人使用
    /// </summary>
    /// <param name="name"></param>
    public void unloadWeak(sCacheUnit scu)
    {
        sCache.GetInstance().clearScu(scu);
    }

    /// <summary>
    /// 读取avatar专用接口
    /// </summary>
    public void loadAvatar(string name, LoadAvatarCallback _callback, bool isAvatar = false)
    {
         sLoadAssetbundle.GetInstance().loadAssetBundle(name, (obj) =>
            {
                if( _callback != null )
                    _callback(obj);
            }, isAvatar);
    }
}
