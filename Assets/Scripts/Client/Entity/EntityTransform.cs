using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTransform : IAvailable
{
    public GameObject EntityObj { get; private set; }
    private Transform trans;
    public Transform Trans
    {
        get
        {
            if (trans == null)
            {
                trans = EntityObj.transform;
            }
            return trans;
        }
    }

    public void SetEntity(GameObject obj)
    {
        EntityObj = obj;
    }

    public bool IsAvailable()//据说好像有 GameObj!=null  但是Equals Null的情况
    {
        return EntityObj != null && !EntityObj.Equals(null);
    }
}
