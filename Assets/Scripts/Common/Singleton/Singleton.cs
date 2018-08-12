public class Singleton<T> : ISingleton where T : class, ISingleton, new()
{
    //单例模式
    protected static T _instance;
    //线程安全使用
    private static readonly object _lockObj = new object();
    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_lockObj)
                {
                    if (null == _instance)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
    public static T InstanceThis()
    {
        return Instance;
    }
}
