﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LuaFramework_AppConstWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(LuaFramework.AppConst), typeof(System.Object));
		L.RegFunction("New", _CreateLuaFramework_AppConst);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegConstant("DebugMode", 1);
		L.RegConstant("UpdateMode", 0);
		L.RegConstant("LuaByteMode", 0);
		L.RegConstant("LuaBundleMode", 0);
		L.RegConstant("TimerInterval", 1);
		L.RegConstant("GameFrameRate", 30);
		L.RegVar("AppName", get_AppName, null);
		L.RegVar("LuaTempDir", get_LuaTempDir, null);
		L.RegVar("ExtName", get_ExtName, null);
		L.RegVar("bundlePath", get_bundlePath, null);
		L.RegVar("AssetDir", get_AssetDir, null);
		L.RegVar("rootName", get_rootName, null);
		L.RegVar("WebUrl", get_WebUrl, null);
		L.RegVar("SocketPort", get_SocketPort, set_SocketPort);
		L.RegVar("SocketAddress", get_SocketAddress, set_SocketAddress);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaFramework_AppConst(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				LuaFramework.AppConst obj = new LuaFramework.AppConst();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: LuaFramework.AppConst.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AppName(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.AppName);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaTempDir(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.LuaTempDir);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ExtName(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.ExtName);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_bundlePath(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.bundlePath);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AssetDir(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.AssetDir);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rootName(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.rootName);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_WebUrl(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.WebUrl);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SocketPort(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, LuaFramework.AppConst.SocketPort);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SocketAddress(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, LuaFramework.AppConst.SocketAddress);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SocketPort(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			LuaFramework.AppConst.SocketPort = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_SocketAddress(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			LuaFramework.AppConst.SocketAddress = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

