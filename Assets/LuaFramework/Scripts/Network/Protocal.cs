public static class Protocal
{
    public const int Connect = 101; //连接服务器
    public const int Exception = 102; //异常掉线
    public const int Disconnect = 103; //正常断线   
    //public const int ConnectFail = 104; //连接服务器

    //public const int Message = 104; //正常接收消息
}

public static class ActionCode
{
    public const int Position = 201;
    public const int Skill = 202;
    public const int Dead = 203;
}