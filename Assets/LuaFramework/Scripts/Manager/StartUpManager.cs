using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaFramework;
using UnityEngine;
using UnityEngine.Networking;

public class StartUpManager : Manager
{
    protected static bool initialize = false;
    private List<string> downloadFiles = new List<string>();

    private DownLoadManager m_DownLoadManager;

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    void Awake()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        //int n = 9;
        //int num = (ushort)((int)(n & 255) << 8 | (n >> 8 & 255));
        //Debug.Log("输出来啦" + num);
        GameObject gameRoot = GameObject.Find("GameRoot");
        if (gameRoot != null)
        {
            GameObject parentGo = gameRoot;
            while (gameRoot.transform.parent != null)
            {
                parentGo = gameRoot.transform.parent.gameObject;
            }
            if (parentGo != null)
            {
                DontDestroyOnLoad(parentGo); //防止销毁自己
            }
        }
        

        CheckExtractResource(); //释放资源

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        string basePath = Util.BuildAssetsPath;
        bool isExists = Directory.Exists(basePath) &&
                        Directory.Exists(basePath + "lua/") &&
                        File.Exists(basePath + "files.txt");

        m_DownLoadManager = new DownLoadManager();
        //Debug.Log(m_DownLoadManager);

        if (isExists || AppConst.DebugMode)
        {
            StartCoroutine(OnUpdateResource());
            return; //文件已经解压过了，自己可添加检查文件列表逻辑
        }

        StartCoroutine(OnExtractResource()); //启动释放协成 
    }

    /// <summary> 
    /// 这里是第一次 释放资源 ,这里所谓的解包，只是复制资源到指定位置而已
    /// 如果一开始就不存在files.txt 是会报错的
    /// </summary>
    IEnumerator OnExtractResource()
    {
        //游戏包资源目录
        //即复制源的位置
        string myPath = Util.BuildAssetsPath;
        if (Directory.Exists(myPath))
            Directory.Delete(myPath, true);
        Directory.CreateDirectory(myPath);

        //files.txt文件
        string filePath = myPath + "files.txt";
        //打包过后，不带有files.txt时，启动下载
        //此处比原生方法多进行了一步下载，但省下了将资源打包进最终程序包里的空间资源
        if (!File.Exists(filePath))
        {
            string fileUrl = AppConst.WebUrl + "files.txt?v=" + DateTime.Now.ToString("yyyymmddhhmmss");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(fileUrl))
            {
                yield return webRequest.SendWebRequest();
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
            }
        }

        yield return new WaitForEndOfFrame();

        //files.txt检测完成，开始启动更新资源
        StartCoroutine(OnUpdateResource());
    }

    /// <summary>
    /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
    /// </summary>
    IEnumerator OnUpdateResource()
    {
        if (!AppConst.UpdateMode)
        {
            OnResourceInited();
            yield break;
        }

        string dataPath = Util.BuildAssetsPath; //数据目录
        string url = AppConst.WebUrl;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string listUrl = url + "files.txt?v=" + random;
        Debug.LogWarning("LoadUpdate---->>>" + listUrl);

        WWW www = new WWW(listUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.Log(www.error);
            //更新失败
            Debug.LogError("更新失败!>");
            yield break;
        }

        File.WriteAllBytes(dataPath + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');

        for (int i = 0; i < files.Length; i++)
        {
            if (string.IsNullOrEmpty(files[i]))
                continue;
            string[] keyValue = files[i].Split('|');
            string f = keyValue[0];
            //本地文件
            string localfile = (dataPath + f).Trim();
            //获取该文件的目录，如果不存在则创建该目录
            string path = Path.GetDirectoryName(localfile);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileUrl = url + f + "?v=" + random;
            bool canUpdate = !File.Exists(localfile);
            if (!canUpdate)
            {
                string remoteMd5 = keyValue[1].Trim();
                string localMd5 = Util.md5file(localfile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate)
                    File.Delete(localfile);
            }

            if (canUpdate)
            {
                //本地缺少文件
                //Debug.Log(fileUrl);

                //这里都是资源文件，用线程下载
                BeginDownload(fileUrl, localfile);
                while (!(IsDownOK(localfile)))
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        yield return new WaitForEndOfFrame();

        OnResourceInited();
    }

    void OnUpdateFailed(string file)
    {
        Debug.Log("更新失败!>" + file);
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

    /// <summary>
    /// 线程下载
    /// </summary>
    void BeginDownload(string url, string file)
    {
        //线程下载
        object[] param = new object[2] {url, file};

        DownLoadEvent ev = new DownLoadEvent();
        ev.State = DownLoadState.Downloading;
        ev.evParams.AddRange(param);
        m_DownLoadManager.AddEvent(ev, OnThreadCompleted);

        //ThreadEvent ev = new ThreadEvent();
        //ev.Key = NotiConst.UPDATE_DOWNLOAD;
        //ev.evParams.AddRange(param);
        //ThreadManager.AddEvent(ev, OnThreadCompleted); //线程下载
    }

    /// <summary>
    /// 线程完成
    /// </summary>
    /// <param name="data"></param>
    void OnThreadCompleted(NewNotiData data)
    {
        switch (data.evState)
        {
            case DownLoadState.Progress: //进度
                //Debug.Log("进度" + data.evParam);
                break;
            case DownLoadState.Done: //下载一个完成
                downloadFiles.Add(data.evParam.ToString());
                Debug.Log("下载完成" + data.evParam);
                break;
        }
    }

    ///// <summary>
    ///// 线程下载
    ///// </summary>
    //void BeginDownload(string url, string file)
    //{
    //    //线程下载
    //    object[] param = new object[2] {url, file};

    //    ThreadEvent ev = new ThreadEvent();
    //    ev.Key = NotiConst.UPDATE_DOWNLOAD;
    //    ev.evParams.AddRange(param);
    //    ThreadManager.AddEvent(ev, OnThreadCompleted); //线程下载
    //}

    ///// <summary>
    ///// 线程完成
    ///// </summary>
    ///// <param name="data"></param>
    //void OnThreadCompleted(NotiData data)
    //{
    //    switch (data.evName)
    //    {
    //        case NotiConst.UPDATE_EXTRACT: //解压一个完成
    //            //
    //            break;
    //        case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
    //            downloadFiles.Add(data.evParam.ToString());
    //            break;
    //    }
    //}

    /// <summary>
    /// 资源初始化结束
    /// </summary>
    public void OnResourceInited()
    {
#if ASYNC_MODE
        //ResManager.Initialize("LuaBundles", delegate()
        ResManager.Initialize(AppConst.AssetDir, delegate()
        {
            Debug.Log("Initialize OK!!!");
            this.OnInitialize();
        });
#else
            ResManager.Initialize();
            this.OnInitialize();
#endif

        //下载完成，释放DownLoadManager
        if (m_DownLoadManager != null)
        {
            m_DownLoadManager.OnStopThread();
            m_DownLoadManager = null;
        }
    }

    void OnInitialize()
    {
        LuaManager.InitStart();

        initialize = true;
    }

    #region Luaframework原生

    ///// <summary>
    ///// 释放资源
    ///// </summary>
    //public void CheckExtractResource()
    //{
    //    bool isExists = Directory.Exists(Util.DataPath) &&
    //                    Directory.Exists(Util.DataPath + "lua/") &&
    //                    File.Exists(Util.DataPath + "files.txt");

    //    if (isExists || AppConst.DebugMode)
    //    {
    //        StartCoroutine(OnUpdateResource());
    //        return; //文件已经解压过了，自己可添加检查文件列表逻辑
    //    }

    //    StartCoroutine(OnExtractResource()); //启动释放协成 
    //}

    ///// <summary> 
    ///// 这里是第一次 释放资源 ,这里所谓的解包，只是复制资源到指定位置而已
    ///// 如果一开始就不存在files.txt 是会报错的
    ///// </summary>
    //IEnumerator OnExtractResource()
    //{
    //    //游戏包资源目录
    //    //即复制源的位置
    //    string resPath = Util.AppContentPath();
    //    //数据目录
    //    //即要复制到的目标路径的位置
    //    string dataPath = Util.DataPath;

    //    if (Directory.Exists(dataPath))
    //        Directory.Delete(dataPath, true);
    //    Directory.CreateDirectory(dataPath);

    //    string infile = resPath + "files.txt";
    //    string outfile = dataPath + "files.txt";
    //    if (File.Exists(outfile))
    //        File.Delete(outfile);

    //    string message /*= "正在解包文件:>files.txt"*/;
    //    Debug.Log(infile);
    //    Debug.Log(outfile);
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        WWW www = new WWW(infile);
    //        yield return www;

    //        if (www.isDone)
    //            File.WriteAllBytes(outfile, www.bytes);

    //        yield return null;
    //    }
    //    else
    //        File.Copy(infile, outfile, true);

    //    yield return new WaitForEndOfFrame();

    //    //释放所有文件到数据目录
    //    string[] files = File.ReadAllLines(outfile);
    //    foreach (var file in files)
    //    {
    //        string[] fs = file.Split('|');
    //        infile = resPath + fs[0]; //
    //        outfile = dataPath + fs[0];

    //        message = "正在解包文件:>" + fs[0];
    //        Debug.Log("正在解包文件:>" + infile);
    //        facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

    //        string dir = Path.GetDirectoryName(outfile);
    //        if (!Directory.Exists(dir))
    //            Directory.CreateDirectory(dir);

    //        if (Application.platform == RuntimePlatform.Android)
    //        {
    //            WWW www = new WWW(infile);
    //            yield return www;

    //            if (www.isDone)
    //            {
    //                File.WriteAllBytes(outfile, www.bytes);
    //            }

    //            yield return null;
    //        }
    //        else
    //        {
    //            if (File.Exists(outfile))
    //            {
    //                File.Delete(outfile);
    //            }

    //            File.Copy(infile, outfile, true);
    //        }

    //        yield return new WaitForEndOfFrame();
    //    }

    //    message = "解包完成!!!";
    //    facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
    //    yield return new WaitForSeconds(0.1f);

    //    //message = string.Empty;
    //    //释放完成，开始启动更新资源
    //    StartCoroutine(OnUpdateResource());
    //}

    ///// <summary>
    ///// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
    ///// </summary>
    //IEnumerator OnUpdateResource()
    //{
    //    if (!AppConst.UpdateMode)
    //    {
    //        OnResourceInited();
    //        yield break;
    //    }

    //    string dataPath = Util.DataPath; //数据目录
    //    string url = AppConst.WebUrl;
    //    string message;
    //    string random = DateTime.Now.ToString("yyyymmddhhmmss");
    //    string listUrl = url + "files.txt?v=" + random;
    //    Debug.LogWarning("LoadUpdate---->>>" + listUrl);

    //    WWW www = new WWW(listUrl);
    //    yield return www;
    //    if (www.error != null)
    //    {
    //        //更新失败
    //        OnUpdateFailed(string.Empty);
    //        yield break;
    //    }

    //    if (!Directory.Exists(dataPath))
    //    {
    //        Directory.CreateDirectory(dataPath);
    //    }

    //    File.WriteAllBytes(dataPath + "files.txt", www.bytes);
    //    string filesText = www.text;
    //    string[] files = filesText.Split('\n');

    //    for (int i = 0; i < files.Length; i++)
    //    {
    //        if (string.IsNullOrEmpty(files[i]))
    //            continue;
    //        string[] keyValue = files[i].Split('|');
    //        string f = keyValue[0];
    //        string localfile = (dataPath + f).Trim();
    //        string path = Path.GetDirectoryName(localfile);
    //        if (!Directory.Exists(path))
    //        {
    //            Directory.CreateDirectory(path);
    //        }

    //        string fileUrl = url + f + "?v=" + random;
    //        bool canUpdate = !File.Exists(localfile);
    //        if (!canUpdate)
    //        {
    //            string remoteMd5 = keyValue[1].Trim();
    //            string localMd5 = Util.md5file(localfile);
    //            canUpdate = !remoteMd5.Equals(localMd5);
    //            if (canUpdate)
    //                File.Delete(localfile);
    //        }

    //        if (canUpdate)
    //        {
    //            //本地缺少文件
    //            Debug.Log(fileUrl);
    //            message = "downloading>>" + fileUrl;
    //            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

    //            //这里都是资源文件，用线程下载
    //            BeginDownload(fileUrl, localfile);
    //            while (!(IsDownOK(localfile)))
    //            {
    //                yield return new WaitForEndOfFrame();
    //            }
    //        }
    //    }

    //    yield return new WaitForEndOfFrame();

    //    message = "更新完成!!";
    //    facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

    //    OnResourceInited();
    //}

    #endregion
}