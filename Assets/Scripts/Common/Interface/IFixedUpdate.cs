
namespace SimpleFramework
{
    /*
     * 游戏循环的更新操作
     */
    public interface IFixedUpdate
    {
        EnumUpdateOrder Order { get; }
        void OnFixedUpdate(float ElapsedSeconds);
    }
}