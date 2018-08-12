using SimpleFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MGame
{
    public class GameStateManager : Singleton<GameStateManager>, IUpdate, IInit
    {
        public const int STATE_CHECK_UPDATE = 1;//检测游戏更新

        //do..

        public const int STATE_INITIAL_CONFIG = 5;//初始化配表

        public const int STATE_START = 6;//开始

        private GameStateControl stateControl = new GameStateControl();

        public EnumUpdateOrder Order { get { return EnumUpdateOrder.Second; } }

        public void OnInit()
        {
            RegisterInterfaceManager.RegisteUpdate(this);

            stateControl.RegisterState<GameStateCheckUpdate>(STATE_CHECK_UPDATE);
            stateControl.RegisterState<GameStateInitialConfig>(STATE_INITIAL_CONFIG);
            stateControl.RegisterState<GameStateStart>(STATE_START);

            SetActiveState(GameStateManager.STATE_CHECK_UPDATE);
        }


        public bool SetActiveState(Int32 stateId, object param = null)
        {
            return stateControl.SetActiveState(stateId, param);
        }
        public void On_Update(float ElapsedSeconds)
        {
            stateControl.On_Update(ElapsedSeconds);
        }

        public void OnRelease()
        {
            RegisterInterfaceManager.UnRegisteUpdate(this);

        }


    }
}