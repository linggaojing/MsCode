using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    public class GameManager : MonoBehaviour
    {
        #region Instance

        //Mono - Singleton
        private static GameManager instance;

        public static GameManager Instance { get { return instance ?? (instance = FindObjectOfType<GameManager>()); } }

        #endregion

        static Dictionary<string, object> m_Managers = new Dictionary<string, object>();

        #region ManagerProperty

        private LuaManager m_LuaMgr;
        private PanelManager m_PanelMgr;
        private ResourceManager m_ResMgr;
        private NetworkManager m_NetMgr;
        private SoundManager m_SoundMgr;
        private TimerManager m_TimerMgr;
        private StartUpManager m_StartUpMgr;
        private LogHandler m_LogMgr;
        //private LogHandler m_LogMgr;

        public LuaManager LuaManager {
            get
            {
                if (m_LuaMgr == null)
                    m_LuaMgr = GetManager<LuaManager>(ManagerName.Lua);
                return m_LuaMgr;
            }
        }

        public PanelManager PanelManager {
            get
            {
                if (m_PanelMgr == null)
                    m_PanelMgr = GetManager<PanelManager>(ManagerName.Panel);
                return m_PanelMgr;
            }
        }

        public ResourceManager ResManager {
            get
            {
                if (m_ResMgr == null)
                    m_ResMgr = GetManager<ResourceManager>(ManagerName.Resource);
                return m_ResMgr;
            }
        }

        public NetworkManager NetManager {
            get
            {
                if (m_NetMgr == null)
                    m_NetMgr = GetManager<NetworkManager>(ManagerName.Network);
                return m_NetMgr;
            }
        }

        public SoundManager SoundManager {
            get
            {
                if (m_SoundMgr == null)
                    m_SoundMgr = GetManager<SoundManager>(ManagerName.Sound);
                return m_SoundMgr;
            }
        }

        public TimerManager TimerManager {
            get
            {
                if (m_TimerMgr == null)
                    m_TimerMgr = GetManager<TimerManager>(ManagerName.Timer);
                return m_TimerMgr;
            }
        }

        //public LogHandler LogManager
        //{
        //get
        //{
        //if (m_LogMgr == null)
        //{
        //m_LogMgr = GetManager<LogHandler>(ManagerName.Log);
        //}
        //return m_LogMgr;
        //}
        //}

        public LogHandler LogManager
        {
            get
            {
                if (m_LogMgr == null)
                {
                    m_LogMgr = GetManager<LogHandler>(ManagerName.Log);
                }
                return m_LogMgr;
            }
        }

        #endregion

        void Start()
        {
            InitManagers();
        }

        private void InitManagers()
        {
            m_LuaMgr = AddManager<LuaManager>(ManagerName.Lua);
            m_PanelMgr = AddManager<PanelManager>(ManagerName.Panel);
            m_ResMgr = AddManager<ResourceManager>(ManagerName.Resource);
            m_NetMgr = AddManager<NetworkManager>(ManagerName.Network);
            m_SoundMgr = AddManager<SoundManager>(ManagerName.Sound);
            m_TimerMgr = AddManager<TimerManager>(ManagerName.Timer);
            m_StartUpMgr = AddManager<StartUpManager>(ManagerName.StartUp);
            m_LogMgr = AddManager<LogHandler>(ManagerName.Log);
        }

        /// <summary>
        /// 添加Unity对象
        /// </summary>
        public T AddManager<T>(string typeName) where T : Component
        {
            object result = null;
            m_Managers.TryGetValue(typeName, out result);
            if (result != null)
            {
                return (T) result;
            }

            Component c = gameObject.AddComponent<T>();
            m_Managers.Add(typeName, c);

            return default(T);
        }

        /// <summary>
        /// 获取系统管理器
        /// </summary>
        public T GetManager<T>(string typeName) where T : class
        {
            if (!m_Managers.ContainsKey(typeName))
            {
                return default(T);
            }

            object manager = null;
            m_Managers.TryGetValue(typeName, out manager);
            return (T) manager;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            if (NetManager != null)
            {
                NetManager.Unload();
            }

            if (LuaManager != null)
            {
                LuaManager.Close();
            }

            Debug.Log("~GameManager was destroyed");
        }
    }
}