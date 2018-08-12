using SimpleFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGame
{
    public class ReadPrototypeManager : Singleton<ReadPrototypeManager>
    {
        private string dataPath = "Data/Config/";

        private Action callBack;
        private List<Type> listTableInfos = new List<Type>();
        public void InitialPrototype(Action cb)
        {
            listTableInfos.Clear();
            callBack = cb;
            ReadDataConfig<CastlePrototype>(dataPath + "Castle/Castle.xml");


        }

        private void ReadDataConfig<T>(string path) where T : BasePrototype
        {
            listTableInfos.Add(typeof(T));
            ResourceManager.LoadAsync(dataPath + "Castle/Castle", obj =>
            {
                if (obj != null)
                {
                    TextAsset textAsset = obj as TextAsset;
                    byte[] b = textAsset.bytes;
                    string content = System.Text.UTF8Encoding.UTF8.GetString(b).Trim();
                    PrototypeManager<T>.Instance.LoadData(content);
                }
                else
                {
                    LogCtrl.Error(" 读取文件失败, Path : " + path + "   Type : " + typeof(T));
                }
                listTableInfos.Remove(typeof(T));
                CheckIsFinish();
            });


        }
        private void CheckIsFinish()
        {
            LogCtrl.Log("  读取配置表，剩余表数量  " + listTableInfos.Count);
            if (listTableInfos.Count == 0)
            {
                ReadFinish();
            }

        }

        public void ReadFinish()
        {
            if (!EventExtension.InvokeGracefully(callBack))
            {
                LogCtrl.Error(" callBack Is Null ");
            }
        }

    }



}
