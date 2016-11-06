using UnityEngine;
using System.Collections;
using sFramework;
using sFramework.LoadBin;
using UnityEngine.SceneManagement;

public class sMapManager : sSingleton<sMapManager>
{
    bool _mapOver = false;
    bool _navOver = false;

    string _mapName = "";
    string _navName = "";

    public void loadMap(int id)
    {
        data_mapinfo tmp = null;
        if (sLoadBin_mapinfo.instance.data.TryGetValue(id, out tmp))
        {
            _mapName = tmp.mapName;
            _navName = tmp.navMesh;
        }

        if (!string.IsNullOrEmpty(_mapName))
            sLoadingGame.GetInstance().loadWeak(_mapName, _loadcallback, false);//生成A
        if (!string.IsNullOrEmpty(_navName))
            sLoadingGame.GetInstance().loadNavmesh(_navName, _loadnavcallback);//生成C-navmesh根本，有了它才能loadscene
    }

    public void unloadMap()
    {
        if (!string.IsNullOrEmpty(_mapName))
            sCache.GetInstance().clearCache(_mapName);//删除A
        if (!string.IsNullOrEmpty(_navName))
        {
            SceneManager.UnloadScene(_navName);//删除B-navmesh scene load
            sLoadingGame.GetInstance().unloadNavmesh(_navName);//删除C
        }
        _mapOver = false;
        _navOver = false;
    }

    public void _loadnavcallback()
    {
        SceneManager.LoadSceneAsync(_navName, LoadSceneMode.Additive);//生成B-navmesh scene load
        _navOver = true;
    }

    public void _loadcallback(sCacheUnit scu)
    {
        //加载场景完成
        //这里暂时将loading隐藏的调用放着，之后流程完成化后需要进flow
        scu.obj.SetActive(true);
        sULoading.instance.hideLoading();   
        _mapOver = true;
    }

    public bool isMapLoadOK()
    {
        return _mapOver && _navOver;
    }
}
