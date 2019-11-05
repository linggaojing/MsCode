using LuaFramework;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private GameManager gameManager;

    private LuaManager m_LuaMgr;
    private ResourceManager m_ResMgr;
    private NetworkManager m_NetMgr;
    private SoundManager m_SoundMgr;
    private TimerManager m_TimerMgr;
    private LogHandler m_LogMgr;
    //private LogHandler m_LogMgr;
    //private ThreadManager m_ThreadMgr;

    protected GameManager GameManager {
        get
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            return gameManager;
        }
    }

    protected LuaManager LuaManager {
        get
        {
            if (m_LuaMgr == null)
            {
                m_LuaMgr = GameManager.GetManager<LuaManager>(ManagerName.Lua);
            }

            return m_LuaMgr;
        }
    }

    protected ResourceManager ResManager {
        get
        {
            if (m_ResMgr == null)
            {
                m_ResMgr = GameManager.GetManager<ResourceManager>(ManagerName.Resource);
            }

            return m_ResMgr;
        }
    }

    protected NetworkManager NetManager {
        get
        {
            if (m_NetMgr == null)
            {
                m_NetMgr = GameManager.GetManager<NetworkManager>(ManagerName.Network);
            }

            return m_NetMgr;
        }
    }

    protected SoundManager SoundManager {
        get
        {
            if (m_SoundMgr == null)
            {
                m_SoundMgr = GameManager.GetManager<SoundManager>(ManagerName.Sound);
            }

            return m_SoundMgr;
        }
    }

    protected TimerManager TimerManager {
        get
        {
            if (m_TimerMgr == null)
            {
                m_TimerMgr = GameManager.GetManager<TimerManager>(ManagerName.Timer);
            }

            return m_TimerMgr;
        }
    }

    protected LogHandler LogManager
    {
        get
        {
            if (m_LogMgr == null)
            {
                m_LogMgr = GameManager.GetManager<LogHandler>(ManagerName.Log);
            }

            return m_LogMgr;
        }
    }

    //protected ThreadManager ThreadManager {
    //    get
    //    {
    //        if (m_ThreadMgr == null)
    //        {
    //            m_ThreadMgr = GameManager.GetManager<ThreadManager>(ManagerName.Thread);
    //        }

    //        return m_ThreadMgr;
    //    }
    //}
}