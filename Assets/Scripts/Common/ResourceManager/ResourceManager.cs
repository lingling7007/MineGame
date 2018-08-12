
using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFramework
{
    public class ResourceManager
    {

        public static Object Load(string path)
        {
            return Resources.Load(path);

        }
        public static void LoadAsync(string path, System.Action<Object> callBack)
        {
            ResourceLoaderAsyn loader = new ResourceLoaderAsyn();
            loader.SetAssetName(path);
            loader.CallBack = callBack;
            EnumeratorTaskManager.Instance.AddTask(DefineEnumeratorTask.ResourceLoadAsyn, loader);

        }


    }

}
