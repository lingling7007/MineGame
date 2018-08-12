using System;
using System.Xml;
using System.Collections.Generic;

public class CastlePrototype : BasePrototype
{
    public int type { get; private set; }
    public int[] range { get; private set; }
    public int level { get; private set; }
    public string modelPath { get; private set; }

    protected override void OnLoadData(XmlNode data)
    {
        type = XmlUtil.GetAttribute<int>(data, "type");
        range = XmlUtil.ParseString<int>(XmlUtil.GetAttribute<string>(data, "range"), new char[] { ',' });
        level = XmlUtil.GetAttribute<int>(data, "level");
        modelPath = XmlUtil.GetAttribute<string>(data, "modelPath");

    }
}