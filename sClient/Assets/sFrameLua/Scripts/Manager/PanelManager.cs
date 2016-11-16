using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LuaInterface;

namespace LuaFramework {
    public class PanelManager : Manager {
        private Transform parent;

        Transform Parent {
            get {
                if (parent == null) {
                    GameObject go = GameObject.FindWithTag("UICamera");
                    if (go != null) parent = go.transform;
                }
                return parent;
            }
        }


#if ASYNC_MODE
        /// <summary>
        /// 创建面板，请求资源管理器
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, LuaFunction func = null) {
            string assetName = name + "Panel";
            string abName = name.ToLower() + AppConst.ExtName;

            ResManager.LoadPrefab(abName, assetName, delegate(UnityEngine.Object[] objs) {
                if (objs.Length == 0) return;
                // Get the asset.
                GameObject prefab = objs[0] as GameObject;

                if (Parent.FindChild(name) != null || prefab == null) {
                    return;
                }
                GameObject go = Instantiate(prefab) as GameObject;
                go.name = assetName;
                go.layer = LayerMask.NameToLayer("Default");
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent<LuaBehaviour>();

                if (func != null) func.Call(go);
                Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
            });
        }
#else
        /// <summary>
        /// 创建面板，请求资源管理器
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, LuaFunction func = null) {
            Debug.Log("create panel:" + name);
            int index = name.LastIndexOf("/");
            string assetName = name.Substring(index + 1);
            GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
            Debug.Log("prefab:" + prefab);
            if (Parent.FindChild(name) != null || prefab == null) {
                return;
            }
            Debug.Log("create ok");
            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<LuaBehaviour>();

            if (func != null) func.Call(go);
            Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
        }
#endif

        private Dictionary<string, Vector3> _pannelInfo = new Dictionary<string, Vector3>();
        public void hideTransformPosition(string name, Transform trans)
        {
            if (!_pannelInfo.ContainsKey(name))
                _pannelInfo.Add(name, trans.localPosition);
            else
                _pannelInfo[name] = trans.localPosition;
            trans.localPosition = trans.localPosition + new Vector3(5000, 0, 0);
        }

        public void showTransformPosition(string name, Transform trans)
        {
            if (_pannelInfo.ContainsKey(name))
            {
                trans.position = _pannelInfo[name];
                _pannelInfo.Remove(name);
            }
            else
                trans.position = Vector3.zero;
        }
    }
}