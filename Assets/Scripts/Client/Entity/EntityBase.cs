using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    private string entityName;
    public string EntityName { get { return entityName; } }
    private EntityTransform transInfo;
    public EntityTransform TransInfo { get { return transInfo; } }

    public void SetEntityName(string name)
    {
        entityName = name;
    }

}
