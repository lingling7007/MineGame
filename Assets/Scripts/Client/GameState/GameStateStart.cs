
using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateStart : GameState
{
    public override void EnterState(GameState preState, object param)
    {
        LogCtrl.Log("  进入 开始游戏  状态 。。");
        MakeEntityManager.Instance.InitialCastle();
        

    }

    public override void LeaveState(GameState nextState)
    {
        LogCtrl.Error("  离开 开始游戏  状态 。。");

    }

    public override void OnUpdate(float deltaTime)
    {

    }
}
