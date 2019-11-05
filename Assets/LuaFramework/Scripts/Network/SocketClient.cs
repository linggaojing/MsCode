using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LuaFramework;
using UnityEngine;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client;
    private NetworkStream outStream;

    private MemoryStream receiveStream;
    private MemoryStream writeStream;
    private BinaryWriter binaryWriter;

    private const int MaxRead = 8192;
    private byte[] byteBuffer = new byte[MaxRead];

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        receiveStream = new MemoryStream();
        writeStream = new MemoryStream();
        binaryWriter = new BinaryWriter(writeStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        this.Close();
        receiveStream.Close();

        writeStream.Close();
        binaryWriter.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }

            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                client = new TcpClient(AddressFamily.InterNetworkV6);
            else
                client = new TcpClient(AddressFamily.InterNetwork);

            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Close();
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        if (client.Connected)
        {
            outStream = client.GetStream();
            client.GetStream().BeginRead(byteBuffer, 0, MaxRead, new AsyncCallback(OnRead), null);
            //通知该连接成功的监听者 
            NetworkManager.AddMessageEvent(Protocal.Connect, null);
        }
        else
        {
            NetworkManager.AddMessageEvent(Protocal.Disconnect, null);
            Close();
        }

        //outStream = client.GetStream();
        //client.GetStream().BeginRead(byteBuffer, 0, MaxRead, new AsyncCallback(OnRead), null);
        ////通知该连接成功的监听者 
        //NetworkManager.AddMessageEvent(Protocal.Connect, null);
    }

    ///// <summary>
    ///// 写数据
    ///// </summary>
    //void WriteMessage(byte[] message)
    //{
    //    MemoryStream ms;
    //    using (ms = new MemoryStream())
    //    {
    //        ms.Position = 0;
    //        BinaryWriter writer = new BinaryWriter(ms);
    //        ushort msglen = (ushort) message.Length;
    //        writer.Write(msglen);
    //        writer.Write(message);
    //        writer.Flush();
    //        Debug.Log(client);
    //        if (client != null && client.Connected)
    //        {
    //            //NetworkStream stream = client.GetStream();
    //            byte[] payload = ms.ToArray();
    //            //for (int i = 0; i < 100; i++)
    //            outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
    //        }
    //        else
    //        {
    //            Debug.LogError("client.connected----->>false");
    //        }
    //    }
    //}

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(int accode, byte[] message)
    {
        binaryWriter.Write((ushort) message.Length);
        binaryWriter.Write((ushort) accode);
        binaryWriter.Write(message);
        binaryWriter.Flush();
        if (client != null && client.Connected)
        {
            byte[] payload = writeStream.ToArray();
            //for (int i = 0; i < 100; i++)
            outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);

            binaryWriter.Seek(0, SeekOrigin.Begin);
            writeStream.SetLength(0);
        }
        else
            Debug.LogError("client.connected----->>false");
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {
                //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }

            if (bytesRead < 1)
            {
                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }

            OnReceive(byteBuffer, bytesRead); //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {
                //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length); //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MaxRead, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Close(); //关掉客户端链接
        int protocal = dis == DisType.Exception ? Protocal.Exception : Protocal.Disconnect;

        //ByteBuffer buffer = new ByteBuffer();
        //buffer.WriteShort((ushort) protocal);
        NetworkManager.AddMessageEvent((ushort) protocal, null);
        Debug.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }

    ///// <summary>
    ///// 打印字节
    ///// </summary>
    ///// <param name="bytes"></param>
    //void PrintBytes()
    //{
    //    string returnStr = string.Empty;
    //    for (int i = 0; i < byteBuffer.Length; i++)
    //    {
    //        returnStr += byteBuffer[i].ToString("X2");
    //    }

    //    Debug.LogError(returnStr);
    //}

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        receiveStream.Seek(0, SeekOrigin.Begin);

        receiveStream.Write(bytes, 0, length);

        int currentIndex = 0;
        while (currentIndex < length)
        {
            ushort messageLen = BitConverter.ToUInt16(bytes, currentIndex);
            //操作码
            ushort actionCode = BitConverter.ToUInt16(bytes, currentIndex + 2);

            //Debug.Log("消息长：" + messageLen + ",当前index：" + currentIndex + ",总长：" + length);

            byte[] compPack = new byte[messageLen];

            //4 = messageLen的字节 + actionCode的字节
            currentIndex += 4;

            receiveStream.Position = currentIndex;

            receiveStream.Read(compPack, 0, messageLen);

            //发送接收到的消息给事件
            OnReceiveMessage(actionCode, compPack);
            currentIndex += messageLen;
        }

        //重置MemoryStream
        receiveStream.SetLength(0);
    }

    /// <summary>
    /// 在这进行组装bytebuffer
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    void OnReceiveMessage(ushort actionCode, byte[] data)
    {
        ByteBuffer bbf = new ByteBuffer(data);

        NetworkManager.AddMessageEvent(actionCode, bbf);
    }

    ///// <summary>
    ///// 接收到消息
    ///// </summary>
    ///// <param name="ms"></param>
    //void OnReceivedMessage(MemoryStream ms)
    //{
    //    BinaryReader r = new BinaryReader(ms);
    //    byte[] message = r.ReadBytes((int) (ms.Length - ms.Position));

    //    ByteBuffer buffer = new ByteBuffer(message);
    //    int mainId = buffer.ReadShort();
    //    NetworkManager.AddEvent(mainId, buffer);
    //}

    ///// <summary>
    ///// 会话发送
    ///// </summary>
    //void SessionSend(byte[] bytes)
    //{
    //    //WriteMessage(bytes);
    //}

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected)
                client.Close();
            client = null;
        }

        //loggedIn = false;
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect()
    {
        ConnectServer(AppConst.SocketAddress, AppConst.SocketPort);
    }

    ///// <summary>
    ///// 发送消息
    ///// </summary>
    //public void SendMessage(ByteBuffer buffer)
    //{
    //    SessionSend(buffer.ToBytes());

    //    buffer.Close();
    //}

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(ByteBuffer buffer)
    {
        WriteMessage(buffer.ProtocalCode, buffer.ToBytes());
    }
}