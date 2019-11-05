using System;
using UnityEngine;

namespace LuaFramework
{
	public class LogManager
	{
		public static bool EnableLog
		{
			get
			{
				return LogManager._EnableLog;
			}
			set
			{
				LogManager._EnableLog = value;
				Debug.unityLogger.logEnabled = LogManager._EnableLog;
			}
		}

		public static void RegisterLogCallback(Action<string, LogType> func)
		{
			LogManager.log_callback = func;
		}

		public static void Log(string message)
		{
			bool enableLog = LogManager.EnableLog;
			if (enableLog)
			{
				bool flag = LogManager.log_callback != null;
				if (flag)
				{
					LogManager.log_callback(message, LogType.Log);
				}
			}
		}

		public static void LogError(string message)
		{
			bool enableLog = LogManager.EnableLog;
			if (enableLog)
			{
				bool flag = LogManager.log_callback != null;
				if (flag)
				{
					LogManager.log_callback(message, LogType.Error);
				}
			}
		}

		public static void LogWarning(string message)
		{
			bool enableLog = LogManager.EnableLog;
			if (enableLog)
			{
				bool flag = LogManager.log_callback != null;
				if (flag)
				{
					LogManager.log_callback(message, LogType.Warning);
				}
			}
		}

		private static Action<string, LogType> log_callback;

		private static bool _EnableLog = false;
	}
}
