using UnityEngine;
using System.Reflection;
using LuaInterface;

namespace LuaFramework
{
    public static class LuaHelper
    {
        /// <summary>
        /// getType
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static System.Type GetType(string classname)
        {
            Assembly assb = Assembly.GetExecutingAssembly(); //.GetExecutingAssembly();
            System.Type t = null;
            t = assb.GetType(classname);
            ;
            if (t == null)
            {
                t = assb.GetType(classname);
            }

            return t;
        }

        /// <summary>
        /// 面板管理器
        /// </summary>
        public static PanelManager GetPanelManager()
        {
            return GameManager.Instance.GetManager<PanelManager>(ManagerName.Panel);
        }

        /// <summary>
        /// 资源管理器
        /// </summary>
        public static ResourceManager GetResManager()
        {
            return GameManager.Instance.GetManager<ResourceManager>(ManagerName.Resource);
        }

        /// <summary>
        /// 网络管理器
        /// </summary>
        public static NetworkManager GetNetManager()
        {
            return GameManager.Instance.GetManager<NetworkManager>(ManagerName.Network);
        }

        /// <summary>
        /// 音乐管理器
        /// </summary>
        public static SoundManager GetSoundManager()
        {
            return GameManager.Instance.GetManager<SoundManager>(ManagerName.Sound);
        }

        /// <summary>
        /// pbc/pblua函数回调
        /// </summary>
        /// <param name="func"></param>
        public static void OnCallLuaFunc(LuaInterface.LuaByteBuffer data, LuaFunction func)
        {
            if (func != null)
                func.Call(data);
            Debug.LogWarning("OnCallLuaFunc length:>>" + data.buffer.Length);
        }

        /// <summary>
        /// cjson函数回调
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnJsonCallFunc(string data, LuaFunction func)
        {
            Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
            if (func != null)
                func.Call(data);
        }
    }
}