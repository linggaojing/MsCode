using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework
{
    public class NetworkManager : Manager
    {
        private SocketClient socket;

        static readonly object m_lockObject = new object();

        private static Queue<KeyValuePair<int, ByteBuffer>> MessageQueue = new Queue<KeyValuePair<int, ByteBuffer>>();

        private event Action<int, ByteBuffer> OnReceiveMessageHandler;
        public event Action<int, ByteBuffer> OnReceiveMessage {
            add
            {
                OnReceiveMessageHandler -= value;
                OnReceiveMessageHandler += value;
            }
            remove { OnReceiveMessageHandler -= value; }
        }

        private LuaFunction luaSocketFunc;

        SocketClient SocketClient {
            get
            {
                if (socket == null)
                    socket = new SocketClient();
                return socket;
            }
        }

        void Awake()
        {
            Init();
        }

        void Init()
        {
            SocketClient.OnRegister();
            OnReceiveMessage += TestMessage;
        }

        //public void OnInit()
        //{
        //    CallMethod("Start");
        //}

        internal void Unload() { }

        ///// <summary>
        ///// 执行Lua方法
        ///// </summary>
        //public object[] CallMethod(string func, params object[] args)
        //{
        //    return Util.CallMethod("Network", func, args);
        //}

        public void RegisterLuaSocket(LuaFunction func)
        {
            if (func != null)
            {
                luaSocketFunc = func;
            }
        }

        public static void AddMessageEvent(ushort accode, ByteBuffer data)
        {
            // lock (m_lockObject)
            //{
            MessageQueue.Enqueue(new KeyValuePair<int, ByteBuffer>(accode, data));
            //}
        }

        private void TestMessage(int accode, ByteBuffer bf)
        {
            switch (accode)
            {
                case Protocal.Connect: break;
                case Protocal.Disconnect: break;
                case Protocal.Exception: break;
            }

            //if (bf != null)
            //{
            //    Debug.Log(bf.ReadString());
            //}

            if (luaSocketFunc != null)
                luaSocketFunc.Call(accode, bf);
        }

        /// <summary>
        /// 交给Command，这里不想关心发给谁。
        /// </summary>
        void Update()
        {
            //if (mEvents.Count > 0)
            //{
            //    while (mEvents.Count > 0)
            //    {
            //        KeyValuePair<int, ByteBuffer> _event = mEvents.Dequeue();
            //        Debug.Log(_event.Value.ReadString());
            //        facade.SendMessageCommand(NotiConst.DISPATCH_MESSAGE, _event);
            //    }
            //}

            if (MessageQueue.Count > 0)
            {
                while (MessageQueue.Count > 0)
                {
                    if (OnReceiveMessageHandler != null)
                    {
                        var ms = MessageQueue.Dequeue();
                        OnReceiveMessageHandler.Invoke(ms.Key, ms.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void SendConnect()
        {
            SocketClient.SendConnect();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        /// <param name="buffer"></param>
        public void SendMessage(ByteBuffer buffer)
        {
            SocketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            SocketClient.OnRemove();
            Debug.Log("~NetworkManager was destroy");
        }
    }
}