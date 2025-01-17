﻿using UnityEngine;
using LuaInterface;
using System;

namespace LuaFramework
{
    public class LuaManager : Manager
    {
        private LuaState lua;
        private LuaLoader loader;
        private LuaLooper loop = null;

        // Use this for initialization
        void Awake()
        {
            //loader = new LuaLoader();
            lua = new LuaState();
            this.OpenLibs();
            lua.LuaSetTop(0);

            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(lua, this);
        }

        public void InitStart()
        {
            //这里从awake转移到这里，因为需要等待资源下载完成再实例化
            loader = new LuaLoader();

            InitLuaPath();
            InitLuaBundle();
            this.lua.Start(); //启动LUAVM
            this.StartMain();
            this.StartLooper();
        }

        void StartLooper()
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        protected void OpenCJson()
        {
            lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.LuaSetField(-2, "cjson");

            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.LuaSetField(-2, "cjson.safe");
        }

        void StartMain()
        {
            lua.DoFile("Main.lua");

            LuaFunction main = lua.GetFunction("Main");
            main.Call();
            main.Dispose();
            main = null;
        }

        /// <summary>
        /// 初始化加载第三方库
        /// </summary>
        void OpenLibs()
        {
            lua.OpenLibs(LuaDLL.luaopen_pb);
            lua.OpenLibs(LuaDLL.luaopen_sproto_core);
            lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);

            this.OpenCJson();
        }

        /// <summary>
        /// 初始化Lua代码加载路径
        /// </summary>
        void InitLuaPath()
        {
            //if (AppConst.DebugMode)
            //{
            //    string rootPath = AppConst.FrameworkRoot;
            //    lua.AddSearchPath(rootPath + "/Lua");
            //    lua.AddSearchPath(rootPath + "/ToLua/Lua");
            //    Debug.Log(rootPath + "/Lua");
            //    Debug.Log(rootPath + "/ToLua/Lua");
            //}
            //else
            //{
            //    lua.AddSearchPath(Util.DataPath + "lua");
            //}
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                string[] paths = Application.dataPath.Split(new string[] { "app" }, StringSplitOptions.None);
                lua.AddSearchPath(paths[0] + "/Lua");
                lua.AddSearchPath(paths[0] + "Assets/LuaFramework/ToLua/Lua");
                return;
            }
            if (AppConst.DebugMode)
            {
                string rootPath = Util.FrameworkRoot;
                lua.AddSearchPath(Application.dataPath.Replace("Assets", "Lua"));
                lua.AddSearchPath(rootPath + "/ToLua/Lua");
            }
            else
            {
                lua.AddSearchPath(Util.BuildAssetsPath + "lua");
            }
        }

        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        void InitLuaBundle()
        {
            if (loader.beZip)
            {
                ////这块开始 是tolua的文件夹
                loader.AddBundle("lua/lua.unity3d");
                loader.AddBundle("lua/lua_math.unity3d");
                loader.AddBundle("lua/lua_system.unity3d");
                loader.AddBundle("lua/lua_system_reflection.unity3d");
                loader.AddBundle("lua/lua_unityengine.unity3d");
                loader.AddBundle("lua/lua_protobuf.unity3d");
                loader.AddBundle("lua/lua_misc.unity3d");
                ////tolua文件夹结束


                loader.AddBundle("lua/lua_common.unity3d");
                loader.AddBundle("lua/lua_logic.unity3d");
                //loader.AddBundle("lua/lua_view.unity3d");
                //loader.AddBundle("lua/lua_controller.unity3d");
                loader.AddBundle("lua/lua_3rd_cjson.unity3d");
                loader.AddBundle("lua/lua_3rd_luabitop.unity3d");
                //loader.AddBundle("lua/lua_3rd_pbc.unity3d");
                //loader.AddBundle("lua/lua_3rd_pblua.unity3d");
                //loader.AddBundle("lua/lua_3rd_sproto.unity3d");
            }
        }

        public void DoFile(string filename)
        {
            lua.DoFile(filename);
        }

        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                return func.LazyCall(args);
            }

            return null;
        }

        public void LuaGC()
        {
            if (lua != null)
            {
                lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
            }
            
        }

        public void Close()
        {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
            loader = null;
        }
    }
}