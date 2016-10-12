using UnityEngine;
using UnityEditor;

public class PrefabLightmapDataEditor : Editor
{
    [MenuItem("Ojcgames Tools/保存该场景预制件的烘焙信息", false, 0)]
    static void SaveLightmapInfoByGameObject()
    {
        GameObject go = Selection.activeGameObject;

        if (null == go) return;

        PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
        if (data == null)
        {
            data = go.AddComponent<PrefabLightmapData>();
        }
        //save lightmapdata info by mesh.render
        data.SaveLightmap();

        EditorUtility.SetDirty(go);
        //applay prefab
        PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go), ReplacePrefabOptions.ConnectToPrefab);
    }
}