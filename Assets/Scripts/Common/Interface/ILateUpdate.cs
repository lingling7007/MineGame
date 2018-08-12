namespace SimpleFramework
{
    /*
     * 游戏循环的更新操作
     */
    public interface ILateUpdate
    {
        EnumUpdateOrder Order { get; }
        void OnLateUpdate(float ElapsedSeconds);
    }
}