using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 生成物体管理器？
/// </summary>
public class MakeEntityManager : Singleton<MakeEntityManager>
{
    //确定地图大小
    public int MaxWidth = 500;
    public int MaxLength = 500;

    public void InitialCastle()
    {
        MakeCastleHelper.InitialCastle(MaxWidth, MaxLength, 10);

    }
}
