using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFramework
{

    public class ResourceLoaderAsyn : BaseResLoader, IEnumeratorTask
    {

        public System.Action<UnityEngine.Object> CallBack;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount = 3;
        /// <summary>
        /// 超时时间
        /// </summary>
        public float TimeOut = 3f;

        private int curRetryCount = 0;
        private DateTime dateTimeOut;
        private ResourceRequest mRequest;
        private WaitForEndOfFrame wait = new WaitForEndOfFrame();


        public virtual short GetState()
        {
            if (mRequest != null)
            {
                if (mRequest.isDone)
                {
                    return ResourceState.Ready;
                }
                return ResourceState.Loading;
            }
            return ResourceState.Waiting;
        }

        public virtual float GetProgress()
        {
            if (mRequest != null)
            {
                return mRequest.progress;
            }
            return 0;
        }

        public IEnumerator DoLoadAsync(Action finishCallback)
        {
            if (mRequest == null)
            {
                StartLoadRes(0);
            }
            while (mRequest != null && curRetryCount < RetryCount)
            {
                if (mRequest.isDone)
                {
                    SetMainAsset(mRequest.asset);
                    EventExtension.InvokeGracefully(finishCallback);
                    EventExtension.InvokeGracefully(CallBack, MainAsset);
                    break;
                }
                if (dateTimeOut > DateTime.Now)
                {
                    StartLoadRes(curRetryCount++);
                }
                yield return wait;
            }
            yield return null;
        }


        public void StartLoadRes(int count)
        {
            mRequest = Resources.LoadAsync(AssetName);
            dateTimeOut = DateTime.UtcNow.AddSeconds(TimeOut);
            curRetryCount = count;
            if (count == 0)
            {
                LogCtrl.Log(" 开始加载 资源 " + AssetName);
            }
            else
            {
                LogCtrl.Log(" 超时重试 加载 资源 " + AssetName + "  当前次数 " + count);
            }
        }

        public override void Release()
        {
            mRequest = null;
            CallBack = null;
        }
    }



}
