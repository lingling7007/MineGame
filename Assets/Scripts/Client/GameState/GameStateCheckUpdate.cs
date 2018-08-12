
using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MGame
{
    public class GameStateCheckUpdate : GameState
    {
        public override void EnterState(GameState preState, object param)
        {
            LogCtrl.Log("  进入 检查 状态 。。");

            GameStateManager.Instance.SetActiveState(GameStateManager.STATE_INITIAL_CONFIG);

        }

        public override void LeaveState(GameState nextState)
        {
            LogCtrl.Log("  离开 检查 状态 。。");

        }

        public override void OnUpdate(float deltaTime)
        {

        }
    }
}