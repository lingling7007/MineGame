
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimpleFramework
{
    public abstract class BaseResLoader : IResLoader
    {
        public string AssetName { get; private set; }
        public UnityEngine.Object MainAsset { get; private set; }

        public void SetAssetName(string assetName)
        {
            AssetName = assetName;
        }
        public void SetMainAsset(UnityEngine.Object asset)
        {
            MainAsset = asset;
        }
        public virtual void Release()
        {
            AssetName = string.Empty;
            MainAsset = null;
        }
    }
}