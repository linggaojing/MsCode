using UnityEngine;
using LuaInterface;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace LuaFramework
{
    public class LuaBehaviour : MonoBehaviour, IPointerClickHandler
    {
        //private string data = null;
        private Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();
        private Dictionary<GameObject, LuaFunction> addClickDir = new Dictionary<GameObject, LuaFunction>();

        private ResourceManager ResManager { get { return GameManager.Instance.ResManager; } }

        private string bindingAbName;
        public string BindingAbName {
            set
            {
                if (value != null && string.Equals(bindingAbName, null))
                {
                    bindingAbName = value;
                }
            }
        }

        /// <summary>
        /// 添加单击事件
        /// </summary>
        public void AddClick(GameObject go, LuaFunction luafunc)
        {
            if (go == null || luafunc == null)
                return;
            buttons.Add(go.name, luafunc);
            LuaFunction tempLuaFunc;
            if (!addClickDir.TryGetValue(go, out tempLuaFunc))
            {
                addClickDir.Add(go, luafunc);
            }

            //go.GetComponent<Button>().onClick.AddListener(() => { luafunc.Call(go); }
            //);
        }

        /// <summary>
        /// 删除单击事件
        /// </summary>
        /// <param name="go"></param>
        public void RemoveClick(GameObject go)
        {
            if (go == null)
                return;
            LuaFunction luafunc = null;
            if (buttons.TryGetValue(go.name, out luafunc))
            {
                luafunc.EndPCall();
                luafunc.Dispose();
                luafunc = null;
                buttons.Remove(go.name);
                addClickDir.Remove(go);
            }
        }

        /// <summary>
        /// 清除单击事件
        /// </summary>
        public void ClearClick()
        {
            foreach (var de in buttons)
            {
                if (de.Value != null)
                {
                    de.Value.Dispose();
                }
            }

            buttons.Clear();
        }

        //-----------------------------------------------------------------
        protected void OnDestroy()
        {
            ClearClick();
#if ASYNC_MODE
            //string abName = name.ToLower().Replace("panel", "");
            ResManager.UnloadAssetBundle(bindingAbName);
#endif
            Util.ClearMemory();
            Debug.Log("~" + name + " was destroy!");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            LuaFunction luaFunc;
            if (addClickDir.TryGetValue(eventData.pointerEnter, out luaFunc))
            {
                luaFunc.Call(eventData.pointerEnter);
            }
        }

        ///// <summary> 
        ///// 以下是原生备份 </summary>
        //        //-----------------------------------------------------------------
        //        protected void OnDestroy()
        //        {
        //            ClearClick();
        //#if ASYNC_MODE
        //            string abName = name.ToLower().Replace("panel", "");
        //            ResManager.UnloadAssetBundle(abName + AppConst.ExtName);
        //#endif
        //            Util.ClearMemory();
        //            Debug.Log("~" + name + " was destroy!");
        //        }
    }
}