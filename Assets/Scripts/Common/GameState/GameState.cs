
/**************************************************************************************************
	Copyright (C) 2016 - All Rights Reserved.
--------------------------------------------------------------------------------------------------------
	当前版本：1.0;
	文	件：GameState.cs;
	注	释：;
**************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GameState
{

    protected Int32 _stateID = -1;

    public void SetStateId(int stateId) { _stateID = stateId; }

    // 获取游戏状态的ID;
    public Int32 GetStateID() { return _stateID; }

	public abstract void EnterState(GameState preState, object param);

    public abstract void LeaveState(GameState nextState);

    public abstract void OnUpdate(float deltaTime);

    public virtual bool TryEnterState(GameState preState) { return true; }

    public virtual bool TryLeaveState(GameState nextState) { return true; }

    public virtual void OnLateUpdate(float deltaTime) { }
}
