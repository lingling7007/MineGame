/**************************************************************************************************
	Copyright (C) 2016 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：GameStateManager.cs;
	注	释：;
**************************************************************************************************/


using SimpleFramework;
using System;
using System.Collections.Generic;

public class GameStateControl
{

    private Dictionary<Int32, GameState> _stateMap = new Dictionary<int, GameState>();
    private GameState _currentState;
    private GameState _preState; // 前一个状态;

    public GameState CurrentState { get { return _currentState; } }
    public GameState PreState { get { return _preState; } }

    public void On_Update(float deltaTime)
    {
        if (_currentState != null)
        {
            _currentState.OnUpdate(deltaTime);
        }
    }

    public void RegisterState<T>(Int32 stateId) where T : GameState, new()
    {
        // 删除原有的注册;
        if (_stateMap.ContainsKey(stateId))
        {
            _stateMap.Remove(stateId);
        }
        T gameState = new T();
        gameState.SetStateId(stateId);
        _stateMap.Add(stateId, gameState);
    }

    public void UnRegisterState(Int32 stateId)
    {
        if (_stateMap == null)
        {
            return;
        }

        if (_stateMap.ContainsKey(stateId))
        {
            _stateMap.Remove(stateId);
        }
    }

    public void SetActivePreState()
    {
        if (_preState == null)
        {
            return;
        }
        SetActiveState(_preState.GetStateID());
    }

    // 设置当前游戏状态;
    public bool SetActiveState(Int32 stateId, object param = null)
    {
        // 别闹;
        if (_currentState != null && _currentState.GetStateID() == stateId)
        {
            return false;
        }
        GameState nextState = null;
        if (!_stateMap.TryGetValue(stateId, out nextState))
        {
            // 如果找不到新状态，就别切换了;
            LogCtrl.Error("切换游戏错误： 要切换的状态不存在 : " + stateId);
            return false;
        }

        bool tryLeaveState = _currentState == null ? true : _currentState.TryLeaveState(nextState);
        bool tryEnterState = nextState.TryEnterState(_currentState);

        // 尝试离开或进入失败，则意味着，当前条件下，不允许切换（至于具体为什么，请查阅详细代码）;
        if (!tryLeaveState || !tryEnterState)
        {
            return false;
        }

        if (_currentState != null)
        {
            _currentState.LeaveState(nextState);
        }

        _preState = _currentState;
        _currentState = nextState;
        _currentState.EnterState(_preState, param);
        return true;
    }

    public Int32 GetCurrentStateId()
    {
        if (_currentState == null)
        {
            return -1;
        }
        return _currentState.GetStateID();
    }
}
