using System;

namespace SimpleFramework
{
    public interface IResLoader
    {
        string AssetName { get; }
        UnityEngine.Object MainAsset { get; }
        void Release();
    }

    public static class ResourceState
    {
        public const short Waiting = 0;
        public const short Loading = 1;
        public const short Ready = 2;
    }
}
