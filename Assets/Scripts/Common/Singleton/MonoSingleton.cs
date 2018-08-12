using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SimpleFramework
{

    public class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
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
                            string parentName = string.Empty;
                            string itemName = string.Empty;

                            MemberInfo info = typeof(T);
                            var attributes = info.GetCustomAttributes(true);
                            foreach (var atribute in attributes)
                            {
                                var defineAttri = atribute as MonoSingletonPath;
                                if (defineAttri == null)
                                {
                                    continue;
                                }


                                break;
                            }


                            GameObject obj = new GameObject(typeof(T).ToString());
                            _instance = obj.AddComponent<T>();


                            GameObject parent = GameObject.Find("[MonoManagerNode]");
                            if (parent == null)
                            {
                                parent = new GameObject("[MonoManagerNode]");
                                UnityEngine.GameObject.DontDestroyOnLoad(parent);
                            }
                            obj.transform.SetParent(parent.transform);
                            //UnityEngine.GameObject.DontDestroyOnLoad(obj);
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

    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
        private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }

    //    var attributes = info.GetCustomAttributes(true);
    //			foreach (var atribute in attributes)
    //			{
    //				var defineAttri = atribute as MonoSingletonPath;
    //				if (defineAttri == null)
    //				{
    //					continue;
    //				}

    //instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, true);
    //				break;
    //			}

}
