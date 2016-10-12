using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void loadAvatarCallback();

public class sAvatarData
{
    public sAvatarControl controller;
    public long playerID;
    public bool isUsing;
    public long leaveTime;
}
public class sAvatarMgr : sSingleton<sAvatarMgr>
{
    

    private List<sAvatarData> _allAvatar = new List<sAvatarData>();
    private Dictionary<long, sAvatarData> _cacheAvatar = new Dictionary<long,sAvatarData>();
    private Dictionary<string, int> _boneCache = new Dictionary<string, int>();

    private long _curLoadID = 0;
    //等待创建列表，avatar需要顺序创建！
    private List<long> _wait2create = new List<long>();

    /// <summary>
    /// 预先读取骨骼，因为mmo通常主角骨骼就那么几个，可以常驻内存
    /// </summary>
    public void preloadBone()
    {
        //todo:
    }


    public sAvatarData createPlayer(long id, string bonename, string weaponname, string[] equipsname, loadAvatarCallback cb)
    {
        if( _boneCache.ContainsKey(bonename))
        {
            ++_boneCache[bonename];
        }
        else
        {
            _boneCache.Add(bonename, 1);
        }
        _wait2create.Add(id);
        sAvatarData ad = new sAvatarData();
        ad.controller = new sAvatarControl(bonename, equipsname, weaponname, true, cb);
        ad.playerID = id;
        ad.isUsing = true;
        ad.leaveTime = 0;
        Debug.LogError("avatar create:" + id);
        _cacheAvatar.Add(id, ad);
        return ad;
    }

    public void update()
    {
        if(_curLoadID == 0 )
        {
            if (_wait2create.Count > 0)
            {
                _curLoadID = _wait2create[0];
                _cacheAvatar[_curLoadID].controller.startLoad();
            }
        }
        else if (_curLoadID > 0)
        {
            if (_cacheAvatar[_curLoadID].controller.allLoadOk)
            {
                _wait2create.RemoveAt(0);
                _curLoadID = 0;
            }
        }
    }

    public void deletePlayer(long id)
    {
        if( _cacheAvatar.ContainsKey(id))
        {
            if( _wait2create.Contains(id))
            {
                _wait2create.Remove(id);
                _cacheAvatar.Remove(id);
                return;
            }
            string bonename = _cacheAvatar[id].controller.skeleton;

            GameObject.Destroy(_cacheAvatar[id].controller.WeaponInstance);
            SkinnedMeshRenderer smr = _cacheAvatar[id].controller.Instance.GetComponent<SkinnedMeshRenderer>();

            GameObject.Destroy(smr.sharedMesh);
            GameObject.Destroy(smr.material.mainTexture);
            GameObject.Destroy(smr.material);

            GameObject.Destroy(_cacheAvatar[id].controller.Instance);
            _cacheAvatar[id].controller.WeaponInstance = null;
            _cacheAvatar[id].controller.Instance = null;
            _cacheAvatar.Remove(id);

            --_boneCache[bonename];
            if( _boneCache[bonename] == 0 )
            {
                //已经没有索引，可以删除
                sLoadAssetbundle.GetInstance().unloadAssetBundle(bonename);
            }
            
        }
    }

    public sAvatarData getPlayer(long id)
    {
        if( _cacheAvatar.ContainsKey(id))
        {
            return _cacheAvatar[id];
        }
        return null;
    }
}
