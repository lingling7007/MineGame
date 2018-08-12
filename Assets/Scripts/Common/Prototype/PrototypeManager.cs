using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using SimpleFramework;

public class PrototypeManager<T> : Singleton<PrototypeManager<T>> where T : BasePrototype
{
    /// <summary>
    /// 以表为单位存储
    /// </summary>
    public Dictionary<Type, Dictionary<int, T>> dicAllTableData = new Dictionary<Type, Dictionary<int, T>>();

    /// <summary>
    /// 读取xml
    /// </summary>
    /// <param name="name">Name.</param>
    public void LoadData( string content)
    {
        try
        {
            //从文件读取到xml
            XmlDocument xmlDoc = new XmlDocument();
            //string xmlString = FileTools.GetResourcesFileToString(pathFile);
            string xmlString = content;//AssetBundleManager.Instance.LoadAsset<TextAsset>(ResourceAssetbundleDefine.PKG_CONFIG, name).text;
            if (xmlString == null || xmlString.Length == 0)
            {
                LogCtrl.Log("是不是版本表配置错误，没找到此表或文件格式不正确！");
                return;
            }
            xmlDoc.LoadXml(xmlString);
            Type refType = null;
            XmlNode node = null;
            //取出表依赖的类
            node = xmlDoc.FirstChild;
            node = node.NextSibling;
            string type = node.Attributes["type"].Value;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            refType = assembly.GetType(type);
            if (refType == null)
            {
                LogCtrl.Log("name = " + typeof(T).ToString() + " 此表可能为新加表，请加入解析类！");
                return;
            }
            //把同一张表所有数据记录
            Dictionary<int, T> dicTempList = new Dictionary<int, T>();
            //解析单条数据
            XmlNodeList nodeList = node.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode childNode = nodeList[i];
                T basePrototype = System.Activator.CreateInstance(refType) as T;
                basePrototype.LoadData(childNode);

                if (dicTempList.ContainsKey(basePrototype.PrototypeId))
                {
                    dicTempList[basePrototype.PrototypeId] = basePrototype;
                }
                else
                {
                    dicTempList.Add(basePrototype.PrototypeId, basePrototype);
                }
            }
            //加入表集合中
            if (dicTempList.Count != 0)
            {
                if (dicAllTableData.ContainsKey(refType))
                {
                    dicAllTableData[refType] = dicTempList;
                }
                else
                {
                    dicAllTableData.Add(refType, dicTempList);
                }
            }
        }
        catch (Exception ex)
        {
            LogCtrl.Error("配置表文件格式错啦！！！！！  name = " + typeof(T).ToString() + " " + ex.ToString());
        }
    }

    /// <summary>
    /// 提取配置属性
    /// </summary>
    /// <returns>The prototype.</returns>
    /// <param name="prototypeID">Prototype I.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetPrototype(int prototypeID)
    {
        T basePrototype = null;

        Type refType = typeof(T);

        Dictionary<int, T> table = GetPrototypeTable();

        if (table != null)
        {
            if (table.TryGetValue(prototypeID, out basePrototype))
            {
                if (basePrototype == null)
                {
                    return null;
                }

                return basePrototype;
            }
            else
            {
#if UNITY_EDITOR
                LogCtrl.Error("此表中没有此ID GetPrototype = " + refType.Name + "   prototypeID = " + prototypeID);
#else
				LogCtrl.Warn("此表中没有此ID GetPrototype = " + refType.Name + "   prototypeID = " + prototypeID);
#endif
            }
        }

        return null;
    }

    private Dictionary<int, T> GetPrototypeTable()
    {
        Type refType = typeof(T);

        if (dicAllTableData.ContainsKey(refType))
        {
            return dicAllTableData[refType];
        }
        else
        {
            LogCtrl.Error("没有" + refType.Name + "表");
        }

        return null;
    }

    public Dictionary<int, T> GetTableDic()
    {
        Type refType = typeof(T);

        Dictionary<int, T> tempList = new Dictionary<int, T>();

        if (dicAllTableData.ContainsKey(refType))
        {
            Dictionary<int, T> temp = dicAllTableData[refType];

            foreach (KeyValuePair<int, T> kv in temp)
            {
                tempList.Add(kv.Key, kv.Value);
            }
        }

        return tempList;
    }

    public List<T> GetTableList()
    {
        Type refType = typeof(T);

        List<T> ls = new List<T>();

        if (dicAllTableData.ContainsKey(refType))
        {
            Dictionary<int, T> temp = dicAllTableData[refType];

            foreach (KeyValuePair<int, T> kv in temp)
            {
                ls.Add(kv.Value);
            }
        }
        return ls;
    }

    /// <summary>
    /// 获取当前表的个数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public int GetTableCount()
    {
        Type refType = typeof(T);
        int count = 0;
        if (dicAllTableData.ContainsKey(refType))
        {
            count = dicAllTableData[refType].Count;
        }

        return count;
    }


}
