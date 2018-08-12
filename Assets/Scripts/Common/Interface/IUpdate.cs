namespace SimpleFramework
{
    public enum EnumUpdateOrder : int
    {
        First = 1,
        Second = 2,
        Third = 3,
        Four = 4,
    }

    /*
     * 游戏循环的更新操作
     */
    public interface IUpdate
    {
        EnumUpdateOrder Order { get; }
        void On_Update(float ElapsedSeconds);
    }
}