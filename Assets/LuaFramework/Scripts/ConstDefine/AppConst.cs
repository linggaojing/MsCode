
namespace LuaFramework
{
    public class AppConst
    {
        public const bool DebugMode = true; //调试模式-用于内部测试

        public const bool UpdateMode = false; //更新模式-默认关闭 

        public const bool LuaByteMode = false; //Lua字节码模式-默认关闭 
        public const bool LuaBundleMode = false; //Lua代码AssetBundle模式

        public const int TimerInterval = 1;
        public const int GameFrameRate = 30; //游戏帧频

        public const string AppName = "LuaMVC"; //应用程序名称
        public const string LuaTempDir = "LuaTemp/"; //临时目录
        public const string ExtName = ".gjab"; //素材扩展名
        public const string bundlePath = "StreamingAssets/abData"; //打包路径
        public const string AssetDir = "abData"; //打包路径的最后一个目录
        public const string rootName = "/LuaFramework";



        //public const string AppPrefix = AppName + "_"; //应用程序前缀
        //public const string AssetDir = "StreamingAssets"; //素材目录 

        public const string WebUrl = "http://localhost:8080/msCode/StreamingAssets/"; //测试更新地址

        //public static string UserId = string.Empty; //用户ID
        public static int SocketPort = 8888; //Socket服务器端口
        public static string SocketAddress = "127.0.0.1"; //Socket服务器地址
    }
}