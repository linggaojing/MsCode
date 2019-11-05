using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

public class NewNotiData
{
    public DownLoadState evState;
    public object evParam;

    public NewNotiData(DownLoadState st, object param)
    {
        this.evState = st;
        this.evParam = param;
    }
}

public class DownLoadEvent
{
    public DownLoadState State;
    public List<object> evParams = new List<object>();
}

public enum DownLoadState
{
    None,
    Downloading,
    Progress,
    Done
}

public class DownLoadManager
{
    private Thread thread;
    private Action<NewNotiData> func;
    private Stopwatch sw;
    private string currDownFile = string.Empty;

    static readonly object m_lockObject = new object();
    static Queue<DownLoadEvent> events = new Queue<DownLoadEvent>();

    public DownLoadManager()
    {
        sw = new Stopwatch();
        thread = new Thread(OnUpdate);
        thread.Start();
    }

    //void Awake()
    //{
    //    //m_SyncEvent = OnSyncEvent;
    //    thread = new Thread(OnUpdate);
    //}

    //// Use this for initialization
    //void Start()
    //{
    //    thread.Start();
    //}

    /// <summary>
    /// 添加到事件队列
    /// </summary>
    /// <param name="ev"></param>
    /// <param name="fc"></param>
    public void AddEvent(DownLoadEvent ev, Action<NewNotiData> fc)
    {
        lock (m_lockObject)
        {
            func = fc;
            events.Enqueue(ev);
        }
    }

    ///// <summary>
    ///// 通知事件
    ///// </summary>
    ///// <param name="data"></param>
    //private void OnSyncEvent(NewNotiData data)
    //{
    //    if (func != null)
    //        func(data); //回调逻辑层
    //}

    // Update is called once per frame
    void OnUpdate()
    {
        while (true)
        {
            lock (m_lockObject)
            {
                if (events.Count > 0)
                {
                    DownLoadEvent e = events.Dequeue();
                    try
                    {
                        switch (e.State)
                        {
                            case DownLoadState.Downloading:
                                OnDownloadFile(e.evParams);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                    }
                }
            }

            Thread.Sleep(1);
        }
    }

    ///// <summary>
    ///// 调用方法
    ///// </summary>
    //void OnExtractFile(List<object> evParams)
    //{
    //    UnityEngine.Debug.LogWarning("Thread evParams: >>" + evParams.Count);

    //    ///------------------通知更新面板解压完成--------------------
    //    NotiData data = new NotiData(NotiConst.UPDATE_DOWNLOAD, null);
    //    if (m_SyncEvent != null)
    //        m_SyncEvent(data);
    //}

    /// <summary>
    /// 下载文件
    /// </summary>
    void OnDownloadFile(List<object> evParams)
    {
        string url = evParams[0].ToString();
        currDownFile = evParams[1].ToString();

        using (WebClient client = new WebClient())
        {
            sw.Start();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            client.DownloadFileAsync(new Uri(url), currDownFile);
        }
    }

    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //double speed = e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds;
        //string speedStr = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

        int value = e.ProgressPercentage;
        NewNotiData ndata = new NewNotiData(DownLoadState.Progress, value);
        if (func != null)
            func(ndata);

        //if (m_SyncEvent != null)
        //    m_SyncEvent(ndata);

        if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive)
        {
            sw.Reset();
            ndata = new NewNotiData(DownLoadState.Done, currDownFile);

            if (func != null)
                func(ndata);

            //if (m_SyncEvent != null)
            //    m_SyncEvent(ndata);
        }
    }

    public void OnStopThread()
    {
        thread.Abort();
        sw.Stop();
        sw = null;
    }

    /// <summary>
    /// 应用程序退出
    /// </summary>
    void OnDestroy()
    {
        thread.Abort();
    }
}