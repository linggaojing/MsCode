using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework
{
    public class PanelManager : Manager
    {
        private Dictionary<string, Transform> ParentColl = new Dictionary<string, Transform>();

        /// <summary> 
        /// 统一的创建方式 </summary>
        private void OnCreateFunc(GameObject prefab, Transform canvasParent, string abName, LuaFunction func)
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(canvasParent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            //var lua = go.AddComponent<LuaBehaviour>();
            //lua.BindingAbName = abName;

            if (func != null)
                func.Call(go);
            Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
        }

        /// <summary> 
        /// 取消了使用默认parent的方法，需要传入canvas名 </summary>
        public void CreatePanel(string abName, string[] panelsName, string canvasName, LuaFunction func = null)
        {
            if (canvasName == null)
                return;

            Transform canvasParent;
            if (!ParentColl.TryGetValue(canvasName, out canvasParent))
            {
                var canvasGroup = GameObject.FindObjectsOfType<Canvas>();
                foreach (var ca in canvasGroup)
                    if (string.Equals(ca.name, canvasName))
                        canvasParent = ca.transform;

                if (canvasParent == null)
                {
                    Debug.LogError("Can't find Canvas:" + canvasName);
                    return;
                }

                ParentColl.Add(canvasName, canvasParent);
            }

            for (int i = 0; i < panelsName.Length; i++)
            {
                string currentPanel = panelsName[i];

                string cAbName = abName.ToLower() + AppConst.ExtName;

#if ASYNC_MODE
                ResManager.LoadPrefab(cAbName, currentPanel, delegate(UnityEngine.Object[] objs)
                {
                    if (objs.Length == 0)
                        return;
                    GameObject prefab = objs[0] as GameObject;
                    if (prefab == null)
                    {
                        //加载失败
                        if (func != null)
                            func.Call<object>(null);
                        return;
                    }

                    OnCreateFunc(prefab, canvasParent, abName, func);
                });
#else
            GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
            if (prefab == null) return;

            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<LuaBehaviour>();

            if (func != null) func.Call(go);
            Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
#endif
            }
        }

        public Transform GetCanvasParent(string canvasName)
        {
            Transform canvasParent = null;
            if (!ParentColl.TryGetValue(canvasName, out canvasParent))
            {
                var canvasGroup = GameObject.FindObjectsOfType<Canvas>();
                foreach (var ca in canvasGroup)
                {
                    if (string.Equals(ca.name, canvasName))
                        canvasParent = ca.transform;
                }

                if (canvasParent == null)
                {
                    Debug.LogError("Can't find Canvas:" + canvasName);
                    return null;
                }

                ParentColl.Add(canvasName, canvasParent);
            }
            return canvasParent;
        }

        /// <summary> 
        /// 取消了使用默认parent的方法，需要传入canvas名 </summary>
        public void CreatePanel(string abName, string panelName, string canvasName, LuaFunction func = null)
        {
            if (canvasName == null)
                return;

            Transform canvasParent = GetCanvasParent(canvasName);
            string cAbName = abName.ToLower() + AppConst.ExtName;

#if ASYNC_MODE
            ResManager.LoadPrefab(cAbName, panelName, delegate(UnityEngine.Object[] objs)
            {
                if (objs.Length == 0)
                    return;
                GameObject prefab = objs[0] as GameObject;
                if (prefab == null)
                {
                    if (!ReferenceEquals(func, null))
                        return;
                    //加载失败
                    if (func != null)
                        func.Call<object>(null);
                    return;
                }

                OnCreateFunc(prefab, canvasParent, abName, func);
            });
#else
            GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
            if (prefab == null) return;

            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<LuaBehaviour>();

            if (func != null) func.Call(go);
            Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
#endif
        }

        public void CreateItem(string abName, string resName, LuaFunction func = null)
        {

            string cAbName = abName.ToLower() + AppConst.ExtName;
            ResManager.LoadPrefab(cAbName, resName, delegate (UnityEngine.Object[] objs)
            {
                if (objs.Length == 0)
                    return;
                GameObject prefab = objs[0] as GameObject;
                if (prefab == null)
                {
                    if (!ReferenceEquals(func, null))
                        return;
                    //加载失败
                    if (func != null)
                        func.Call<object>(null);
                    return;
                }
                else
                {
                    GameObject go = Instantiate(prefab);
                    if (func != null)
                        func.Call<GameObject>(go);
                }
            });
        }

            #region LuaFramework原生代码

        private Transform parent;

        Transform Parent {
            get
            {
                if (parent == null)
                {
                    GameObject go = GameObject.FindWithTag("GuiCamera");

                    if (go != null)
                        parent = go.transform;
                }

                return parent;
            }
        }

        /// <summary>
        /// ������壬������Դ������
        /// </summary>
        /// <param name="type"></param>
        [Obsolete]
        public void CreatePanel(string name, LuaFunction func = null)
        {
            string assetName = name + "Panel";
            string abName = name.ToLower() + AppConst.ExtName;
            if (Parent.Find(name) != null)
                return;

#if ASYNC_MODE
            ResManager.LoadPrefab(abName, assetName, delegate(UnityEngine.Object[] objs)
            {
                if (objs.Length == 0)
                    return;
                GameObject prefab = objs[0] as GameObject;
                if (prefab == null)
                    return;

                GameObject go = Instantiate(prefab) as GameObject;
                go.name = assetName;
                go.layer = LayerMask.NameToLayer("Default");
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;

                go.AddComponent<LuaBehaviour>();

                if (func != null)
                    func.Call(go);
                Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
            });
            //同步模式没有进行过测试
#else
            GameObject prefab = ResManager.LoadAsset<GameObject>(name, assetName);
            if (prefab == null) return;

            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<LuaBehaviour>();

            if (func != null) func.Call(go);
            Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
#endif
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        [Obsolete]
        public void ClosePanel(string name)
        {
            var panelName = name + "Panel";
            var panelObj = Parent.Find(panelName);
            if (panelObj == null)
                return;
            Destroy(panelObj.gameObject);
        }

        #endregion
    }
}