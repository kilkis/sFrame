using UnityEngine;
using System.Collections;
using sFrame.LoadBin;
public class sLoadBin : sSingleton<sLoadBin>
{

	public override void Init()
    {
        Debug.Log("path:" + sLoadAssetbundle.GetInstance().getBinFilePath("mapinfo.bin"));
        sLoadBin_mapinfo.instance.load(sLoadAssetbundle.GetInstance().getBinFilePath("mapinfo.bin"));
    }

}
