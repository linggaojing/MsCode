using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;

namespace LuaFramework
{
    /// <summary>
    /// 集成自LuaFileUtils，重写里面的ReadFile，
    /// </summary>
    public class LuaLoader : LuaFileUtils
    {
        //保存所有
        private Dictionary<string, string> AllLuaBundlesPathColl;

        //private ResourceManager m_resMgr;

        //ResourceManager resMgr {
        //    get { 
        //        if (m_resMgr == null)
        //            m_resMgr = AppFacade.Instance.GetManager<ResourceManager>(ManagerName.Resource);
        //        return m_resMgr;
        //    }
        //}

        // Use this for initialization
        public LuaLoader()
        {
            instance = this;
            beZip = AppConst.LuaBundleMode;
            AllLuaBundlesPathColl = new Dictionary<string, string>();
            InitPathsColl();
        }

        private void InitPathsColl()
        {
            //string[] folders = Directory.GetDirectories(AppConst.BuildAssetsPath + "lua");
            string[] files = Directory.GetFiles(Util.BuildAssetsPath + "lua");
            foreach (var file in files)
            {
                if (file.EndsWith(AppConst.ExtName))
                {
                    string k = file.Replace("\\", "/").Split('/').Last().Replace(AppConst.ExtName, "");

                    AllLuaBundlesPathColl.Add(k, file);
                }
            }
        }

        /// <summary>
        /// 添加打入Lua代码的AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        public void AddBundle(string bundleName)
        {
            //string url = Util.DataPath + bundleName.ToLower();
            string url = Util.BuildAssetsPath + bundleName.ToLower();
            if (File.Exists(url))
            {
                var bytes = File.ReadAllBytes(url);
                AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
                if (bundle != null)
                {
                    //bundleName = bundleName.Replace("lua/", "").Replace(".unity3d", "");
                    bundleName = bundleName.Replace("lua/", "").Replace(AppConst.ExtName, "");
                    base.AddSearchBundle(bundleName.ToLower(), bundle);
                }
            }
        }

        ///// <summary>
        ///// 当LuaVM加载Lua文件的时候，这里就会被调用，
        ///// 用户可以自定义加载行为，只要返回byte[]即可。
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public override byte[] ReadFile(string fileName) {

        //    return base.ReadFile(fileName);     
        //}    

        /// <summary>
        /// 当LuaVM加载Lua文件的时候，这里就会被调用，
        /// 用户可以自定义加载行为，只要返回byte[]即可。
        /// 此部分已被重写
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            if (!beZip)
            {
                string path = FindFile(fileName);
                byte[] str = null;

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
#if !UNITY_WEBPLAYER
                    str = File.ReadAllBytes(path);
#else
                    throw new LuaException("can't run in web platform, please switch to other platform");
#endif
                }

                return str;
            }
            else
            {
                return ReadZipFile(fileName);
            }
        }


        /// <summary> 
        /// 这里重写了 </summary>
        private byte[] ReadZipFile(string fileName)
        {
            AssetBundle zipFile;
            byte[] buffer = null;
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append("lua");
                int pos = fileName.LastIndexOf('/');

                if (pos > 0)
                {
                    sb.Append("_");
                    sb.Append(fileName, 0, pos).ToLower().Replace('/', '_');
                    fileName = fileName.Substring(pos + 1);
                }

                if (!fileName.EndsWith(".lua"))
                {
                    fileName += ".lua";
                }

#if UNITY_5 || UNITY_5_3_OR_NEWER
                fileName += ".bytes";
#endif
                var zipName = sb.ToString();
                //重写部分，如果没查找到，则去文件夹查找
                zipMap.TryGetValue(zipName, out zipFile);
                if (!zipMap.TryGetValue(zipName, out zipFile))
                {
                    string path;
                    if (AllLuaBundlesPathColl.TryGetValue(zipName, out path))
                    {
                        var bytes = File.ReadAllBytes(path);
                        AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
                        if (bundle != null)
                        {
                            base.AddSearchBundle(zipName, bundle);
                            zipFile = bundle;
                        }
                    }
                }
            }

            if (zipFile != null)
            {
#if UNITY_4_6 || UNITY_4_7
                TextAsset luaCode = zipFile.Load(fileName, typeof(TextAsset)) as TextAsset;
#else
                TextAsset luaCode = zipFile.LoadAsset<TextAsset>(fileName);
#endif
                if (luaCode != null)
                {
                    buffer = luaCode.bytes;
                    Resources.UnloadAsset(luaCode);
                }
            }

            return buffer;
        }
    }
}