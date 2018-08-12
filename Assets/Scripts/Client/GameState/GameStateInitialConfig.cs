
using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MGame
{
    public class GameStateInitialConfig : GameState
    {
        public override void EnterState(GameState preState, object param)
        {
            LogCtrl.Log("  进入 更新配置 状态 。。");
            ReadPrototypeManager.Instance.InitialPrototype(() =>
            {


                GameStateManager.Instance.SetActiveState(GameStateManager.STATE_START);

            });

        }

        public override void LeaveState(GameState nextState)
        {
            LogCtrl.Log("  离开 更新配置 状态 。。");

        }

        public override void OnUpdate(float deltaTime)
        {

        }

    }
}